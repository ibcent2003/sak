using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class ProductManagementController : Controller
    {

        private PROEntities db = new PROEntities();
        private ProcessUtility util = new ProcessUtility();
        public ActionResult Index()
        {
            try
            {
                ProductManagementViewModel model = new ProductManagementViewModel();
                var getProduct = db.Product.ToList();
                model.ProductList = getProduct;
                model.documentPath = Properties.Settings.Default.FullPhotoPath;
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult NewProduct()
        {
            try
            {                              
                ProductManagementViewModel model = new ProductManagementViewModel();                    
                model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();                
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewProduct(ProductManagementViewModel model)
        {
            try
            {              
                if (ModelState.IsValid)
                {
                    //check for duplicate

                    var validate = db.Product.Where(x => x.Name == model.Productform.Name && x.RawMaterialId==model.Productform.RawMaterialId).ToList();
                    if (validate.Any())
                    {
                        TempData["message"] = "The Name " + model.Productform.Name + " already exist. Please enter different name";
                        TempData["messageType"] = "danger";

                        model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        return View(model);
                    }
                    string url = Properties.Settings.Default.FullPhotoPath;
                    System.IO.Directory.CreateDirectory(url);

                    if (null == model.Productform.Photo1)
                    {
                        model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "ERROR: You have to upload at least photo 1 to proceed.";
                        model.documentPath = Properties.Settings.Default.FullPhotoPath;
                        return View(model);

                    }

                    CodeGenerator CodePhoto = new CodeGenerator();
                    #region upload Photo1

                    int max_upload = 5242880;


                    List<DocumentInfo> uploadedPhoto1 = new List<DocumentInfo>();

                   
                    string EncKey1 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<DocumentFormat> Photo1types = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                    List<string> supportedPhoto1 = new List<string>();
                    foreach (var item in Photo1types)
                    {
                        supportedPhoto1.Add(item.Extension);
                    }
                    var filePhoto1 = System.IO.Path.GetExtension(model.Productform.Photo1.FileName);
                    if (!supportedPhoto1.Contains(filePhoto1))
                    {
                        model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();

                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPhoto1) + " are supported for Photo 1";
                        model.documentPath = Properties.Settings.Default.FullPhotoPath;
                        return View(model);

                    }
                    else if (model.Productform.Photo1.ContentLength > max_upload)
                    {

                        model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Photo 1 uploaded is larger than the 5MB upload limit";
                        model.documentPath = Properties.Settings.Default.FullPhotoPath;
                        return View(model);
                    }
                    int pp = 0;
                    string pName;
                    pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.Productform.Photo1.FileName);
                    model.Productform.Photo1.SaveAs(url + pName);

                    #endregion

                    if (model.Productform.Photo2 != null && model.Productform.Photo2.ContentLength > 0)
                    {
                        CodeGenerator CodePhoto2 = new CodeGenerator();

                        #region upload Photo2

                        int max_upload2 = 5242880;

                        string EncKey2 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                        List<DocumentFormat> Photo2types = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                        List<string> supportedPhoto2 = new List<string>();
                        foreach (var item in Photo2types)
                        {
                            supportedPhoto2.Add(item.Extension);
                        }
                        var filePhoto2 = System.IO.Path.GetExtension(model.Productform.Photo2.FileName);
                        if (!supportedPhoto2.Contains(filePhoto2))
                        {
                            model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();

                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPhoto2) + " are supported for Photo 2";
                            model.documentPath = Properties.Settings.Default.FullPhotoPath;
                            return View(model);
                        }
                        else if (model.Productform.Photo2.ContentLength > max_upload2)
                        {
                            model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            TempData["messageType"] = "danger";
                            TempData["message"] = "The Photo 2 uploaded is larger than the 5MB upload limit";
                            model.documentPath = Properties.Settings.Default.FullPhotoPath;
                            return View(model);
                        }                       
                        int pp2 = 0;
                        string pName2;
                        pName2 = EncKey2 + pp2.ToString() + System.IO.Path.GetExtension(model.Productform.Photo2.FileName);
                        model.Productform.Photo2.SaveAs(url + pName2);
                        model.p2 = pName2;

                        #endregion
                    }

                    Product add = new Product
                    {
                        Name = model.Productform.Name,
                        ProductCode = "STN" + CodePhoto.PadZeroes(6, CodePhoto.GetNextID("Stones")),
                        DiscountPrice = model.Productform.DiscountPrice,
                        ActualPrice = model.Productform.AcutalPrice,
                        Quantity = model.Productform.Quantity,
                        ReoderLevel = model.Productform.ReorderLevel,
                        RawMaterialId = model.Productform.RawMaterialId,
                       
                        Photo1 = pName,
                        Photo2 = model.p2,
                      
                        Description = model.Productform.Description,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = model.Productform.IsDeleted
                    };
                    db.Product.AddObject(add);
                    
                    db.SaveChanges();
                    TempData["message"] = "" + model.Productform.Name + " has been added successful.";
                    return RedirectToAction("Index", "ProductManagement", new {area = "Setup" });

                }
                model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                TempData["message"] = "Ops! Something went wrong. Please make sure you enter all fields with red sign.";
                TempData["messageType"] = "danger";
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult EditProduct(int Id)
        {
            try
            {              
                ProductManagementViewModel model = new ProductManagementViewModel();                
                var product = db.Product.Where(x => x.Id == Id).FirstOrDefault();               
                model.Productform = new ProductForm();
                model.Productform.Name = product.Name;
                model.Productform.ProductCode = product.ProductCode;
                model.Productform.RawMaterialId = product.RawMaterial.Id;
                model.Productform.DiscountPrice = product.DiscountPrice;
                model.Productform.AcutalPrice = product.ActualPrice;
                model.Productform.Quantity = product.Quantity;
                model.Productform.ReorderLevel = product.ReoderLevel;
              
                model.p1 = product.Photo1;
                if (product.Photo2 != null)
                {
                    model.p2 = product.Photo2;
                }                               
                model.Productform.Id = Id;
                model.Productform.Description = product.Description;
                model.Productform.IsDeleted = product.IsDeleted;
               
                model.documentPath = Properties.Settings.Default.FullPhotoPath;
                model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditProduct(ProductManagementViewModel model)
        {
            try
            {
                var product = db.Product.Where(x => x.Id == model.Productform.Id).FirstOrDefault();
                model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                if (ModelState.IsValid)
                {
                    //var product = db.Product.Where(x => x.Id == model.Productform.Id).FirstOrDefault();
                    if (product == null)
                    {
                        TempData["message"] = Settings.Default.GenericExceptionMessage;
                        TempData["messageType"] = "danger";
                        return RedirectToAction("Index");
                    }

                    model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                    string url = Properties.Settings.Default.FullPhotoPath;
                    System.IO.Directory.CreateDirectory(url);

                    if (model.Productform.Photo1 != null && model.Productform.Photo1.ContentLength > 0)
                    {
                        #region upload Photo1

                        int max_upload = 5242880;

                        CodeGenerator CodePhoto1 = new CodeGenerator();
                        string EncKey1 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                        List<DocumentFormat> Photo1types = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                        List<string> supportedPhoto1 = new List<string>();
                        foreach (var item in Photo1types)
                        {
                            supportedPhoto1.Add(item.Extension);
                        }
                        var filePhoto1 = System.IO.Path.GetExtension(model.Productform.Photo1.FileName);
                        if (!supportedPhoto1.Contains(filePhoto1.ToLower()))
                        {

                            model.p1 = product.Photo1;
                            if (product.Photo2 != null)
                            {
                                model.p2 = product.Photo2;
                            }
                            
                            model.documentPath = Properties.Settings.Default.FullPhotoPath;

                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPhoto1) + " are supported for Photo 1";
                            model.documentPath = Properties.Settings.Default.FullPhotoPath;
                            return View(model);

                        }
                        else if (model.Productform.Photo1.ContentLength > max_upload)
                        {

                            model.p1 = product.Photo1;
                            if (product.Photo2 != null)
                            {
                                model.p2 = product.Photo2;
                            }
                            
                            model.documentPath = Properties.Settings.Default.FullPhotoPath;

                            TempData["messageType"] = "danger";
                            TempData["message"] = "The Photo 1 uploaded is larger than the 5MB upload limit";
                            model.documentPath = Properties.Settings.Default.FullPhotoPath;
                            return View(model);
                        }

                        if (product.Photo1 != null)
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(url + product.Photo1);
                            fi.Delete();
                        }

                        //store product
                        int pp = 0;
                        string pName;
                        pName = EncKey1 + pp.ToString() + System.IO.Path.GetExtension(model.Productform.Photo1.FileName);
                        model.Productform.Photo1.SaveAs(url + pName);
                        product.Photo1 = pName;

                        #endregion
                    }

                    if (model.Productform.Photo2 != null && model.Productform.Photo2.ContentLength > 0)
                    {
                        #region upload Photo2

                        int max_upload2 = 5242880;

                        CodeGenerator CodePhoto2 = new CodeGenerator();
                        string EncKey2 = util.MD5Hash(DateTime.Now.Ticks.ToString());
                        List<DocumentFormat> Photo2types = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                        List<string> supportedPhoto2 = new List<string>();
                        foreach (var item in Photo2types)
                        {
                            supportedPhoto2.Add(item.Extension);
                        }
                        var filePhoto2 = System.IO.Path.GetExtension(model.Productform.Photo2.FileName);
                        if (!supportedPhoto2.Contains(filePhoto2.ToLower()))
                        {
                            model.p1 = product.Photo1;
                            if (product.Photo2 != null)
                            {
                                model.p2 = product.Photo2;
                            }
                            model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                            model.documentPath = Properties.Settings.Default.FullPhotoPath;
                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedPhoto2) + " are supported for Photo 2";
                            model.documentPath = Properties.Settings.Default.FullPhotoPath;
                            return View(model);
                        }
                        else if (model.Productform.Photo2.ContentLength > max_upload2)
                        {

                            model.p1 = product.Photo1;
                            if (product.Photo2 != null)
                            {
                                model.p2 = product.Photo2;
                            }
                           
                            model.documentPath = Properties.Settings.Default.FullPhotoPath;
                            TempData["messageType"] = "danger";
                            TempData["message"] = "The Photo 2 uploaded is larger than the 5MB upload limit";
                            model.documentPath = Properties.Settings.Default.FullPhotoPath;
                            return View(model);
                        }

                        if (product.Photo2 != null)
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(url + product.Photo2);
                            fi.Delete();
                        }

                        //store product
                        int pp2 = 0;
                        string pName2;
                        pName2 = EncKey2 + pp2.ToString() + System.IO.Path.GetExtension(model.Productform.Photo2.FileName);
                        model.Productform.Photo2.SaveAs(url + pName2);
                        model.p2 = pName2;
                        product.Photo2 = pName2;

                        #endregion
                    }
                   
                    product.Name = model.Productform.Name;
                   // product.ProductCode = model.Productform.ProductCode;
                    product.DiscountPrice = model.Productform.DiscountPrice;
                    product.ActualPrice = model.Productform.AcutalPrice;
                    product.ReoderLevel = model.Productform.ReorderLevel;
                    product.RawMaterialId = model.Productform.RawMaterialId;
                   
                    product.Description = model.Productform.Description;
                    product.IsDeleted = model.Productform.IsDeleted;
                    product.ModifiedBy = User.Identity.Name;
                    product.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "" + model.Productform.Name + " has been updated successful.";
                    return RedirectToAction("Index", "ProductManagement", new { area = "Setup" });
                }

                TempData["message"] = "Ops! Something went wrong. Please make sure you enter all fields with red sign.";
                TempData["messageType"] = "danger";

                model.ProductTypeList = (from s in db.RawMaterial where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                model.p1 = product.Photo1;
                if (product.Photo2 != null)
                {
                    model.p2 = product.Photo2;
                }
               
                model.documentPath = Properties.Settings.Default.FullPhotoPath;
                return View(model);


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult DocumentsUploadedPath(string path)
        {
            try
            {
                var filepath = new Uri(path);
                if (System.IO.File.Exists(filepath.AbsolutePath))
                {
                    byte[] filedata = System.IO.File.ReadAllBytes(filepath.AbsolutePath);
                    string contentType = MimeMapping.GetMimeMapping(filepath.AbsolutePath);

                    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = path,
                        Inline = true,
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(filedata, contentType);
                }
                else
                {
                    return null;
                    throw new Exception("ERROR: System could not generate report.");
                }
            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public ActionResult ReorderProduct(int Id)
        {
            try
            {
                

                ProductManagementViewModel model = new ProductManagementViewModel();
               
                var product = db.Product.Where(x => x.Id == Id).FirstOrDefault();
                if (product == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Store", new { area = "Setup" });
                }
                model.Productform = new ProductForm();
                //model.Productform.Name = product.Name;
                //model.Productform.Quantity = product.Quantity;
                model.p1 = product.Photo1;
                if (product.Photo2 != null)
                {
                    model.p2 = product.Photo2;
                }
                
                model.Productform.Id = Id;
                
                model.product = product;
                model.documentPath = Properties.Settings.Default.FullPhotoPath;

                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult ReorderProduct(ProductManagementViewModel model)
        {
            try
            {
               
               

                var product = db.Product.Where(x => x.Id == model.Productform.Id).FirstOrDefault();
                if (product == null)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
               

                if (model.Productform.NewQuantity == 0)
                {
                    TempData["message"] = "ERROR: Please enter the New Quantity you are redordering.";
                    TempData["messageType"] = "danger";

                   
                    model.p1 = product.Photo1;
                    if (product.Photo2 != null)
                    {
                        model.p2 = product.Photo2;
                    }
                   
                    model.documentPath = Properties.Settings.Default.FullPhotoPath;
                    model.product = product;
                    return View(model);

                }

               
                model.p1 = product.Photo1;
                if (product.Photo2 != null)
                {
                    model.p2 = product.Photo2;
                }
               
                int old_qty = product.Quantity;
                int new_qty = model.Productform.NewQuantity;
                int total_qty = old_qty + new_qty;
                product.Quantity = total_qty;
                ReorderHistory addnew = new ReorderHistory
                {
                    ProductId = product.Id,
                    OldQuantity = product.Quantity,
                    NewQuantity = model.Productform.NewQuantity,
                    DiscountPrice = product.DiscountPrice,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now
                };
                db.ReorderHistory.AddObject(addnew);
                db.SaveChanges();
                model.documentPath = Properties.Settings.Default.FullPhotoPath;
                model.product = product;
                TempData["message"] = "" + product.Name + " has been Re-Order successful.";
                return RedirectToAction("Index", "ProductManagement", new {area = "Setup" });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult RedoerHistoryList(int Id)
        {
            try
            {
                ProductManagementViewModel model = new ProductManagementViewModel();
                if(Id==0)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var getProduct = db.Product.Where(X=>X.Id==Id).ToList();
                
                model.ProductList = getProduct;
                var getproduct = db.Product.Where(X => X.Id == Id).FirstOrDefault();
                var reorder = getproduct.ReorderHistory.ToList();
                model.ReorderList = reorder;
                model.product = getproduct;
                model.documentPath = Properties.Settings.Default.FullPhotoPath;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }
        public ActionResult PrintReorderReport(int Id)
        {
            try
            {
                ProductManagementViewModel model = new ProductManagementViewModel();
                if (Id == 0)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var getProduct = db.Product.Where(X => X.Id == Id).ToList();

                model.ProductList = getProduct;
                var getproduct = db.Product.Where(X => X.Id == Id).FirstOrDefault();
                var reorder = getproduct.ReorderHistory.ToList();
                model.ReorderList = reorder;
                model.product = getproduct;
                model.documentPath = Properties.Settings.Default.FullPhotoPath;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult ReorderByDate(int Id)
        {
            try
            {
                System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("en-US");
                ProductManagementViewModel model = new ProductManagementViewModel();
                if (Id == 0)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                var getProduct = db.Product.Where(X => X.Id == Id).ToList();

                model.ProductList = getProduct;
                var getproduct = db.Product.Where(X => X.Id == Id).FirstOrDefault();
                if(model.searchForm != null)
                {
                    model.product = getproduct;
                    model.documentPath = Properties.Settings.Default.FullPhotoPath;

                    var reorder = getproduct.ReorderHistory.Where(x=>x.CreatedDate == model.searchForm.FromDate && x.CreatedDate == model.searchForm.ToDate).ToList();
                    model.ReorderList = reorder;
                    model.HasSearch = true;
                    return View(model);

                }
                model.HasSearch = false;
                model.product = getproduct;
                model.documentPath = Properties.Settings.Default.FullPhotoPath;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult ReorderByDate(ProductManagementViewModel model)
        {
            try
            {
                System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("en-US");
                var getProduct = db.Product.Where(X => X.Id == model.product.Id).ToList();

                model.ProductList = getProduct;
                var getproduct = db.Product.Where(X => X.Id == model.product.Id).FirstOrDefault();
                model.product = getproduct;
                model.documentPath = Properties.Settings.Default.FullPhotoPath;

                var reorder = getproduct.ReorderHistory.Where(s=>s.CreatedDate == model.searchForm.FromDate && s.CreatedDate == model.searchForm.ToDate).ToList();
                model.ReorderList = reorder;
                model.HasSearch = true;
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult ReorderList()
        {
            try
            {
                ProductManagementViewModel model = new ProductManagementViewModel();
                var getProduct = db.Product.Where(x => x.ReoderLevel >= x.Quantity).ToList();
                model.ProductList = getProduct;
                model.documentPath = Properties.Settings.Default.FullPhotoPath;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


    }
}
