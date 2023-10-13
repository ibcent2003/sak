using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class TruckController : Controller
    {
        private PROEntities db = new PROEntities();
        private ProcessUtility util;

        public TruckController()
        {
            this.util = new ProcessUtility();
           
        }
        public ActionResult Index()
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                model.TruckList = db.Truck.ToList();
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
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

        public ActionResult NewTruck()
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
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
        public ActionResult NewTruck(TruckViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string url = Settings.Default.FullPhotoPath;
                    System.IO.Directory.CreateDirectory(url);

                    var validate = db.Truck.Where(x => x.Name == model.Truckform.Name).ToList();
                    if (validate.Any())
                    {
                        TempData["message"] = "ERROR: The Truck name " + model.Truckform.Name + " already exist. Please enter different name.";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    else
                    {

                        if (model.Truckform.Photo != null && model.Truckform.Photo.ContentLength > 0)
                        {
                            #region upload document

                            int max_upload = 5242880;                          

                            UI.Models.CodeGenerator CodePassport = new UI.Models.CodeGenerator();
                            string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                            List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                            List<string> supportedResume = new List<string>();
                            foreach (var item in Resumetypes)
                            {
                                supportedResume.Add(item.Extension);
                            }
                            var fileResume = System.IO.Path.GetExtension(model.Truckform.Photo.FileName);
                            if (!supportedResume.Contains(fileResume.ToLower()))
                            {                               
                                TempData["messageType"] = "danger";
                                TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for truck Photo ";
                                return View(model);

                            }

                            if (model.Truckform.Photo.ContentLength > max_upload)
                            {
                                
                                TempData["messageType"] = "danger";
                                TempData["message"] = "The Photo uploaded is larger than the 5MB upload limit";
                                return View(model);
                            }
                            #endregion

                            #region save Resume
                            int i = 0;
                            string filename;
                            filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.Truckform.Photo.FileName);
                            model.Truckform.Photo.SaveAs(url + filename);
                            #endregion
                            Truck addnew = new Truck
                            {
                                Name = model.Truckform.Name,
                                Number = model.Truckform.Number,
                                IsDeleted = model.Truckform.IsDeleted,
                                ModifiedBy = User.Identity.Name,
                                ModifiedDate = DateTime.Now,
                                Photo = filename
                            };
                            db.Truck.AddObject(addnew);
                            db.SaveChanges();
                            TempData["message"] = "<b>" + model.Truckform.Name + "</b> Type has been added successful";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Truck addnew = new Truck
                            {
                                Name = model.Truckform.Name,
                                Number = model.Truckform.Number,
                                IsDeleted = model.Truckform.IsDeleted,
                                ModifiedBy = User.Identity.Name,
                                ModifiedDate = DateTime.Now,
                                Photo = "truck.jpg"
                            };
                            db.Truck.AddObject(addnew);
                            db.SaveChanges();
                            TempData["message"] = "<b>" + model.Truckform.Name + "</b> Type has been added successful";
                            return RedirectToAction("Index");
                        }                         
                    }

                }
                TempData["message"] = "There is Error while adding truck";
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

        public ActionResult EditTruck(int Id)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                var getTruck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                if(getTruck == null)
                {
                    TempData["message"] = "There is Error while editing truck. Please please entry again.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.Truckform = new  TruckForm();
                model.Truckform.Id = Id;
                model.Truckform.Name = getTruck.Name;
                model.Truckform.Number = getTruck.Number;
                model.Truckform.IsDeleted = getTruck.IsDeleted;
                model.UploadedPhoto = getTruck.Photo;
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
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
        public ActionResult EditTruck(TruckViewModel model)
        {
            try
            {
                    var GetTruck = db.Truck.Where(x => x.Id == model.Truckform.Id).FirstOrDefault();
                    string url = Settings.Default.FullPhotoPath;
                    System.IO.Directory.CreateDirectory(url);
                    if (model.Truckform.Photo != null && model.Truckform.Photo.ContentLength > 0)
                    {
                    if (GetTruck.Photo != null)
                    {
                        if (GetTruck.Photo == "truck.jpg")
                        {

                        }
                        else
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(url + GetTruck.Photo);
                            fi.Delete();
                        }

                    }

                    #region upload document
                    int max_upload = 5242880;
                    UI.Models.CodeGenerator CodePassport = new UI.Models.CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();
                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.Truckform.Photo.FileName);
                    if (!supportedResume.Contains(fileResume.ToLower()))
                    {
                       
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for truck phone ";
                        return View(model);

                    }
                    if (model.Truckform.Photo.ContentLength > max_upload)
                    {
                        
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The truck photo uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    int pp = 0;
                    string pName;
                    pName = EncKey + pp.ToString() + System.IO.Path.GetExtension(model.Truckform.Photo.FileName);
                    model.Truckform.Photo.SaveAs(url + pName);
                    #endregion

                    GetTruck.Name = model.Truckform.Name;
                    GetTruck.Number = model.Truckform.Number;
                    GetTruck.IsDeleted = model.Truckform.IsDeleted;
                    GetTruck.ModifiedBy = User.Identity.Name;
                    GetTruck.ModifiedDate = DateTime.Now;
                    GetTruck.Photo = pName;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.Truckform.Name + "</b> has been updated successful";
                    return RedirectToAction("Index");

                }
                else
                    {
                    GetTruck.Name = model.Truckform.Name;
                    GetTruck.Number = model.Truckform.Number;
                    GetTruck.IsDeleted = model.Truckform.IsDeleted;
                    GetTruck.ModifiedBy = User.Identity.Name;
                    GetTruck.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.Truckform.Name + "</b> has been updated successful";
                    return RedirectToAction("Index");

                }                                                       
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult TruckDashboard(int Id)
        {
            try
            {

                TruckViewModel model = new TruckViewModel();
                var getTruck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                if(getTruck==null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }
                model.EmployeeList = getTruck.Employee.ToList();
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
                model.truck = getTruck;
                model.TruckMaterial = getTruck.RawMaterial.ToList();
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

        public ActionResult AssignMaterial(int Id)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                var getTruck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;

                model.truck = getTruck;                
                List<int> list = (from x in getTruck.RawMaterial select x.Id).ToList<int>();
                model.AvailableMaterial = (from d in this.db.RawMaterial where !list.Contains(d.Id) && d.IsDeleted == false select d).ToList<RawMaterial>();
                model.TruckMaterial = getTruck.RawMaterial.ToList<RawMaterial>();
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

        [HttpPost]
        public ActionResult AssignMaterial(TruckViewModel model)
        {
            try
            {
                var getTruck = db.Truck.Where(x => x.Id == model.truck.Id).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("TruckDashboard");
                }
                model.truck = getTruck;
                List<int> list = (from x in getTruck.RawMaterial select x.Id).ToList<int>();
                model.AvailableMaterial = (from d in this.db.RawMaterial where !list.Contains(d.Id) && d.IsDeleted == false select d).ToList<RawMaterial>();
                model.TruckMaterial = getTruck.RawMaterial.ToList<RawMaterial>();
                if (model.MaterialId == 0)
                {                   
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Please select a Material from the dropdownlist and click on the Add button";
                    return RedirectToAction("AssignMaterial", "Truck", new { Id = model.truck.Id, area = "Setup" });
                }
                if (!(from x in getTruck.RawMaterial where x.Id == model.MaterialId select x).ToList<RawMaterial>().Any<RawMaterial>())
                {
                    RawMaterial material = (from x in this.db.RawMaterial where x.Id == model.MaterialId select x).FirstOrDefault<RawMaterial>();
                    getTruck.RawMaterial.FirstOrDefault<RawMaterial>();
                    getTruck.RawMaterial.Add(material);
                    this.db.SaveChanges();
                  
                    List<int> listed = (from x in getTruck.RawMaterial select x.Id).ToList<int>();
                    model.AvailableMaterial = (from d in this.db.RawMaterial where !listed.Contains(d.Id) && d.IsDeleted == false select d).ToList<RawMaterial>();
                    model.TruckMaterial = getTruck.RawMaterial.ToList<RawMaterial>();


                    base.TempData["message"] = "The material has been added successfully.";
                    RedirectToAction("AssignMaterial", "Truck", new { Id = model.truck.Id, area = "Setup" });
                }
                else
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "The Selected material already exist. Please select another material";
                    return View(model);
                }
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

        public ActionResult RemoveMaterial(int Id, int materialId)
        {
            ActionResult action;
            try
            {
                TruckViewModel model = new TruckViewModel();
                var truck = (from d in this.db.Truck where d.Id == Id select d).FirstOrDefault();
                var material = (from x in this.db.RawMaterial where x.Id == materialId select x).FirstOrDefault();
                truck.RawMaterial.Remove(material);
                this.db.SaveChanges();
                base.TempData["message"] = "The material has been deleted successfully.";
                action = base.RedirectToAction("AssignMaterial", "Truck", new { Id = Id, area = "Setup" });
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
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
                    //throw new Exception("ERROR: System could not generate report.");
                }
            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public ActionResult AssignDriver(int Id)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                var getTruck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;

                model.truck = getTruck;
                List<int> list = (from x in getTruck.Employee where x.EmployeeTypeId==Properties.Settings.Default.EmployeeTypeId select x.Id).ToList<int>();
                model.AvailableEmployee = (from d in this.db.Employee where !list.Contains(d.Id) && d.IsDeleted == false select d).ToList<Employee>();
                model.EmployeeList = getTruck.Employee.Where(x=>x.EmployeeTypeId==Properties.Settings.Default.EmployeeTypeId).ToList();
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
        public ActionResult AssignDriver(TruckViewModel model)
        {
            try
            {              
                var getTruck = db.Truck.Where(x => x.Id == model.truck.Id).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;

                model.truck = getTruck;
                List<int> list = (from x in getTruck.Employee where x.EmployeeTypeId == Properties.Settings.Default.EmployeeTypeId select x.Id).ToList<int>();
                model.AvailableEmployee = (from d in this.db.Employee where !list.Contains(d.Id) && d.IsDeleted == false select d).ToList<Employee>();
                model.EmployeeList = getTruck.Employee.Where(x => x.EmployeeTypeId == Properties.Settings.Default.EmployeeTypeId).ToList();

                if (model.EmployeeId == 0)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Please select a driver and click on the Add button";
                    return RedirectToAction("AssignDriver", "Truck", new { Id = model.truck.Id, area = "Setup" });
                }
                if (!(from x in getTruck.Employee where x.Id == model.EmployeeId && x.EmployeeTypeId==Properties.Settings.Default.EmployeeTypeId select x).ToList<Employee>().Any<Employee>())
                {
                    Employee employee = (from x in this.db.Employee where x.Id == model.EmployeeId && x.EmployeeTypeId==Properties.Settings.Default.EmployeeTypeId select x).FirstOrDefault<Employee>();
                    getTruck.Employee.FirstOrDefault<Employee>();
                    getTruck.Employee.Add(employee);
                    this.db.SaveChanges();

                    List<int> listed = (from x in getTruck.Employee select x.Id).ToList<int>();
                    model.AvailableEmployee = (from d in this.db.Employee where !listed.Contains(d.Id) && d.IsDeleted == false select d).ToList<Employee>();
                    model.EmployeeList = getTruck.Employee.ToList<Employee>();

                    base.TempData["message"] = "The Driver has been added successfully.";
                    RedirectToAction("AssignDriver", "Truck", new { Id = model.truck.Id, area = "Setup" });
                }

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
        public ActionResult RemoveDriver(int Id, int EmployeeId)
        {
            ActionResult action;
            try
            {
                TruckViewModel model = new TruckViewModel();
                var truck = (from d in this.db.Truck where d.Id == Id select d).FirstOrDefault();
                var driver = (from x in this.db.Employee where x.Id == EmployeeId select x).FirstOrDefault();
                truck.Employee.Remove(driver);
                this.db.SaveChanges();
                base.TempData["message"] = "The driver has been deleted successfully.";
                action = base.RedirectToAction("AssignDriver", "Truck", new { Id = Id, area = "Setup" });
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        public ActionResult TruckExpense(int Id)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                var getTruck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
                model.truck = getTruck;               
                model.TruckExpenseList = getTruck.TruckExpense.ToList();
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

        public ActionResult NewTruckExpense(int Id)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                var getTruck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
                model.truck = getTruck;
                model.TruckExpenseList = getTruck.TruckExpense.ToList();
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
        public ActionResult NewTruckExpense(TruckViewModel model)
        {
            try
            {

                var getTruck = db.Truck.Where(x => x.Id == model.truck.Id).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }              
                model.truck = getTruck;
                if (ModelState.IsValid)
                {
                    string url = Settings.Default.FullPhotoPath;
                    System.IO.Directory.CreateDirectory(url);

                    if (model.TruckExpenseform.Receipt != null && model.TruckExpenseform.Receipt.ContentLength > 0)
                    {



                        #region upload document

                        int max_upload = 5242880;

                        UI.Models.CodeGenerator CodePassport = new UI.Models.CodeGenerator();
                        string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                        List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                        List<string> supportedResume = new List<string>();
                        foreach (var item in Resumetypes)
                        {
                            supportedResume.Add(item.Extension);
                        }
                        var fileResume = System.IO.Path.GetExtension(model.TruckExpenseform.Receipt.FileName);
                        if (!supportedResume.Contains(fileResume.ToLower()))
                        {
                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for Receipt ";
                            return View(model);

                        }

                        if (model.TruckExpenseform.Receipt.ContentLength > max_upload)
                        {

                            TempData["messageType"] = "danger";
                            TempData["message"] = "The receipt uploaded is larger than the 5MB upload limit";
                            return View(model);
                        }
                        #endregion

                        #region save Resume
                        int i = 0;
                        string filename;
                        filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.TruckExpenseform.Receipt.FileName);
                        model.TruckExpenseform.Receipt.SaveAs(url + filename);
                        #endregion

                        TruckExpense addnew = new TruckExpense
                        {
                            Description = model.TruckExpenseform.Description,
                            TruckId = getTruck.Id,
                            Amount = model.TruckExpenseform.Amount,
                            ExpenseDate = model.TruckExpenseform.ExpenseDate,
                            IsDeleted = model.TruckExpenseform.IsDeleted,
                            ModifiedBy = User.Identity.Name,
                            ModifedDate = DateTime.Now,                            
                            Receipt = filename,
                            AddedDate = DateTime.Today.ToShortDateString()
                        };
                        db.TruckExpense.AddObject(addnew);
                        db.SaveChanges();
                        TempData["message"] = "<b>" + model.TruckExpenseform.Description + "</b> has been added successful";
                     
                        return RedirectToAction("TruckExpense", "Truck", new { Id= getTruck.Id, area = "Setup" });
                    }
                    else
                    {
                        TruckExpense addnew = new TruckExpense
                        {
                            Description = model.TruckExpenseform.Description,
                            TruckId = getTruck.Id,
                            Amount = model.TruckExpenseform.Amount,
                            ExpenseDate = model.TruckExpenseform.ExpenseDate,
                            IsDeleted = model.TruckExpenseform.IsDeleted,
                            ModifiedBy = User.Identity.Name,
                            ModifedDate = DateTime.Now,
                            AddedDate = DateTime.Today.ToShortDateString()
                        };
                        db.TruckExpense.AddObject(addnew);
                        db.SaveChanges();
                        TempData["message"] = "<b>" + model.TruckExpenseform.Description + "</b> has been added successful";
                        return RedirectToAction("TruckExpense", "Truck", new { Id = getTruck.Id, area = "Setup" });
                    }
                }
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

        public ActionResult EditTruckExpense(int TruckId, int Id)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                var getTruck = db.Truck.Where(x => x.Id == TruckId).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }
                model.truck = getTruck;
                var getexpense = getTruck.TruckExpense.Where(x => x.Id == Id).FirstOrDefault();
                model.TruckExpense = getexpense;
                model.TruckExpenseform = new TruckExpenseForm();
                model.TruckExpenseform.Id = Id;
                model.truck.Id = TruckId;
                model.TruckExpenseform.Description = getexpense.Description;
                model.TruckExpenseform.Amount = getexpense.Amount;
                model.TruckExpenseform.IsDeleted = getexpense.IsDeleted;
                model.UploadedPhoto = getexpense.Receipt;
                model.TruckExpenseform.ExpenseDate = getexpense.ExpenseDate;
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
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

        [HttpPost]
        public ActionResult EditTruckExpense(TruckViewModel model)
        {
            try
            {
                var getTruck = db.Truck.Where(x => x.Id == model.truck.Id).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Ops: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }
                var getexpense = db.TruckExpense.Where(x => x.TruckId == getTruck.Id && x.Id == model.TruckExpenseform.Id).FirstOrDefault();
                if (getexpense == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Ops: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }
                model.TruckExpense = getexpense;
                model.truck = getTruck;
               
                    string url = Settings.Default.FullPhotoPath;
                    System.IO.Directory.CreateDirectory(url);
                    
                    
                    if (model.TruckExpenseform.Receipt != null && model.TruckExpenseform.Receipt.ContentLength > 0)
                    {                      
                        if (getexpense.Receipt != null)
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(url + getexpense.Receipt);
                            fi.Delete();
                        }

                        #region upload document

                        int max_upload = 5242880;

                        UI.Models.CodeGenerator CodePassport = new UI.Models.CodeGenerator();
                        string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                        List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                        List<string> supportedResume = new List<string>();
                        foreach (var item in Resumetypes)
                        {
                            supportedResume.Add(item.Extension);
                        }
                        var fileResume = System.IO.Path.GetExtension(model.TruckExpenseform.Receipt.FileName);
                        if (!supportedResume.Contains(fileResume.ToLower()))
                        {
                            TempData["messageType"] = "danger";
                            TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for Receipt ";
                            return View(model);

                        }

                        if (model.TruckExpenseform.Receipt.ContentLength > max_upload)
                        {

                            TempData["messageType"] = "danger";
                            TempData["message"] = "The receipt uploaded is larger than the 5MB upload limit";
                            return View(model);
                        }
                        #endregion

                        #region save Resume
                        int i = 0;
                        string filename;
                        filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.TruckExpenseform.Receipt.FileName);
                        model.TruckExpenseform.Receipt.SaveAs(url + filename);
                        #endregion
                        getexpense.Description = model.TruckExpenseform.Description;
                        getexpense.Amount = model.TruckExpenseform.Amount;
                        getexpense.ExpenseDate = model.TruckExpenseform.ExpenseDate;
                        getexpense.IsDeleted = model.TruckExpenseform.IsDeleted;
                        getexpense.ModifiedBy = User.Identity.Name;
                        getexpense.ModifedDate = DateTime.Now;
                        getexpense.Receipt = filename;
                        db.SaveChanges();
                        TempData["message"] = "<b>" + model.TruckExpenseform.Description + "</b> has been saved successful";
                        return RedirectToAction("TruckExpense", "Truck", new { Id = getTruck.Id, area = "Setup" });
                    }
                    getexpense.Description = model.TruckExpenseform.Description;
                    getexpense.Amount = model.TruckExpenseform.Amount;
                    getexpense.ExpenseDate = model.TruckExpenseform.ExpenseDate;
                    getexpense.IsDeleted = model.TruckExpenseform.IsDeleted;
                    getexpense.ModifiedBy = User.Identity.Name;
                    getexpense.ModifedDate = DateTime.Now;                   
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.TruckExpenseform.Description + "</b> has been saved successful";
                    return RedirectToAction("TruckExpense", "Truck", new { Id = getTruck.Id, area = "Setup" });

                
                
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult DeleteExpense(int TruckId, int Id)
        {
            ActionResult action;
            try
            {
                TruckViewModel model = new TruckViewModel();
                string url = Settings.Default.FullPhotoPath;
                System.IO.Directory.CreateDirectory(url);
                var getTruck = db.Truck.Where(x => x.Id == TruckId).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }

                var expense = (from x in db.TruckExpense where x.Id == Id && x.TruckId==TruckId select x).FirstOrDefault();
                if(null == expense)
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "Ops! There is error updating expense. Please try again later.";
                    action = base.RedirectToAction("TruckExpense", "Truck", new { Id = TruckId, area = "Setup" });
                }
                if (expense.Receipt != null)
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(url + expense.Receipt);
                    fi.Delete();
                }
                TruckExpense del = new TruckExpense
                {

                };
                db.TruckExpense.DeleteObject(expense);
                this.db.SaveChanges();
                base.TempData["message"] = "The Expense has been deleted successfully.";
                action = base.RedirectToAction("TruckExpense", "Truck", new { Id = TruckId, area = "Setup" });
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        public ActionResult SearchExpense(int Id)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                var getTruck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                if (getTruck == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Op: Something went wrong. Please try again";
                    return RedirectToAction("Index");
                }
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
                model.truck = getTruck;
               // model.TruckExpenseList = getTruck.TruckExpense.ToList();
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


        public ActionResult TruckList()
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                model.TruckList = db.Truck.ToList();
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
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


        public ActionResult ExpensesDecision(int Id)
        {
            try
            {
               
                var getActiveExpenses = db.ExpensesType.Where(x => x.TruckId == Id && x.IsEnded == false).FirstOrDefault();
                if(getActiveExpenses != null)
                {
                    return RedirectToAction("RecordExp", "Truck", new { Id = Id, ExpTypeId= getActiveExpenses.Id, area = "Setup" });
                }
                else
                {
                    return RedirectToAction("NewExpensesType", "Truck", new { Id = Id,  area = "Setup" });
                }
               
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult NewExpensesType(int Id)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                model.TruckList = db.Truck.Where(x=>x.Id==Id).ToList();
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
                model.ExpenesTypeform = new ExpenesTypeForm();
                model.ExpenesTypeform.TruckId = Id;
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

        [HttpPost]
        public ActionResult NewExpensesType(TruckViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var validate = db.ExpensesType.Where(x => x.ExpMonth == model.ExpenesTypeform.ExpMonth && x.TruckId==model.ExpenesTypeform.TruckId).ToList();
                    if(validate.Any())
                    {
                        TempData["message"] = "Error: The Expenses Month & Year exist. Please enter another Month & Year";
                        TempData["messageType"] = "danger";
                        model.TruckList = db.Truck.Where(x => x.Id == model.ExpenesTypeform.TruckId).ToList();
                        model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
                        return View(model);
                    }
                    ExpensesType addnew = new ExpensesType
                    {
                        ExpMonth = model.ExpenesTypeform.ExpMonth,
                        OpenBalance = model.ExpenesTypeform.OpeningBalance,
                        TruckId = model.ExpenesTypeform.TruckId,
                        IsEnded = model.ExpenesTypeform.HasEnded,
                        IsLoan = model.ExpenesTypeform.IsLoan
                    };
                    db.ExpensesType.AddObject(addnew);
                    db.SaveChanges();
                    if(model.ExpenesTypeform.IsLoan==true)
                    {

                        decimal cr = 0;
                        decimal deb = model.ExpenesTypeform.OpeningBalance;
                        decimal total = cr - deb;

                        Expenses exp = new Expenses
                        {
                            ExpensesTypeId = addnew.Id,
                            TruckId = model.ExpenesTypeform.TruckId,
                            Description = model.ExpenesTypeform.Description,
                            Debit = model.ExpenesTypeform.OpeningBalance,
                            Credit = 0,
                            TotalBalance = total,
                            ExpensesDate = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                            ModifiedDate = DateTime.Now
                        };
                        db.Expenses.AddObject(exp);
                        db.SaveChanges();
                    }
                    else
                    {
                        Expenses exp = new Expenses
                        {
                            ExpensesTypeId = addnew.Id,
                            TruckId = model.ExpenesTypeform.TruckId,
                            Description = model.ExpenesTypeform.Description,
                            Debit = 0,
                            Credit = model.ExpenesTypeform.OpeningBalance,
                            TotalBalance = model.ExpenesTypeform.OpeningBalance,
                            ExpensesDate = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                            ModifiedDate = DateTime.Now
                        };
                        db.Expenses.AddObject(exp);
                        db.SaveChanges();
                    }
                    TempData["message"] = "The Expenses for " + model.ExpenesTypeform.ExpMonth + ". have been created successfully.";
                    return RedirectToAction("RecordExp", "Truck", new { Id = model.ExpenesTypeform.TruckId, ExpTypeId = addnew.Id, area = "Setup" });

                }
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                model.TruckList = db.Truck.Where(x => x.Id == model.ExpenesTypeform.TruckId).ToList();
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
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


        public ActionResult RecordExp(int Id, int ExpTypeId)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                model.truck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                model.Expensestype = db.ExpensesType.Where(x => x.Id == ExpTypeId && x.TruckId == Id).FirstOrDefault();
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
                model.ExpensesList = db.Expenses.Where(x => x.ExpensesTypeId == ExpTypeId && x.TruckId == Id).OrderBy(x=>x.Id).ToList();

                var getExp = db.Expenses.Where(x => x.ExpensesTypeId == ExpTypeId).OrderByDescending(x => x.Id).FirstOrDefault();
                if(getExp != null)
                {
                    model.expenserecordForm = new ExpensesRecordForm();
                    model.expenserecordForm.TotalBalance = getExp.TotalBalance;
                }
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

        [HttpPost]
        public ActionResult RecordNewExp(TruckViewModel model)
        {
            try
            {

                if (model.expenserecordForm.Credit != 0)
                {
                    decimal bal = model.expenserecordForm.TotalBalance;
                    decimal? cr = model.expenserecordForm.Credit;
                    decimal totalbalance = bal + cr.Value;

                    Expenses addnew = new Expenses
                    {
                        ExpensesTypeId = model.Expensestype.Id,
                        TruckId = model.truck.Id,
                        Description = model.expenserecordForm.Description,
                        Debit = 0,
                        Credit = model.expenserecordForm.Credit,
                        TotalBalance = totalbalance,
                        ExpensesDate = model.expenserecordForm.ExpensesDate,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    db.Expenses.AddObject(addnew);
                    db.SaveChanges();


                    TempData["message"] = "The Expenses has been recorded successfully.";
                    return RedirectToAction("RecordExp", "Truck", new { Id = model.truck.Id, ExpTypeId = model.Expensestype.Id, area = "Setup" });

                }
                {

                    decimal bal = model.expenserecordForm.TotalBalance;
                    decimal? deb = model.expenserecordForm.Debit;
                    decimal totalbalance = bal - deb.Value;

                    Expenses addnew = new Expenses
                    {
                        ExpensesTypeId = model.Expensestype.Id,
                        TruckId = model.truck.Id,
                        Description = model.expenserecordForm.Description,
                        Debit = model.expenserecordForm.Debit,
                        Credit = 0,
                        TotalBalance = totalbalance,
                        ExpensesDate = model.expenserecordForm.ExpensesDate,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    db.Expenses.AddObject(addnew);
                     db.SaveChanges();

                   // TempData["message"] = "Debit Enter is entered. Total is " + totalbalance + " ";
                    TempData["message"] = "The Expenses has been recorded successfully.";
                    return RedirectToAction("RecordExp", "Truck", new { Id = model.truck.Id, ExpTypeId = model.Expensestype.Id, area = "Setup" });

                }
                    
                    
               
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult EditRecordExp(int Id)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                var getexp = db.Expenses.Where(x => x.Id == Id).FirstOrDefault();
                model.expenserecordForm = new ExpensesRecordForm();
                model.expenserecordForm.Id = getexp.Id;
                model.expenserecordForm.Description = getexp.Description;
                model.expenserecordForm.Debit = getexp.Debit;
                model.expenserecordForm.Credit = getexp.Credit;
                model.expenserecordForm.TotalBalance = getexp.TotalBalance;
                model.expenserecordForm.ExpensesDate = getexp.ExpensesDate;
                model.expenses = getexp;

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

        [HttpPost]
        public ActionResult EditRecordExp(TruckViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var getexp = db.Expenses.Where(x => x.ExpensesTypeId == 10).OrderByDescending(x=>x.Id).Take(2).ToList();
                    if(getexp.Count ==0)
                    {
                        TempData["message"] = Settings.Default.GenericExceptionMessage;                       
                        return RedirectToAction("RecordExp", "Truck", new { Id = getexp.FirstOrDefault().TruckId, ExpTypeId =getexp.FirstOrDefault().ExpensesTypeId, area = "Setup" });
                    }
                    if(getexp.FirstOrDefault().Credit != 0)
                    {

                        decimal bal = getexp.FirstOrDefault().TotalBalance;
                        decimal? cr = model.expenserecordForm.Credit;
                        decimal totalbalance = bal + cr.Value;

                        getexp.FirstOrDefault().Description = model.expenserecordForm.Description;
                        getexp.FirstOrDefault().Debit = 0;
                        getexp.FirstOrDefault().Credit = model.expenserecordForm.Credit;
                        getexp.FirstOrDefault().TotalBalance = totalbalance;
                        getexp.FirstOrDefault().ExpensesDate = model.expenserecordForm.ExpensesDate;
                        getexp.FirstOrDefault().ModifiedBy = User.Identity.Name;
                        getexp.FirstOrDefault().ModifiedDate = DateTime.Now;
                      //  db.SaveChanges();
                    }
                    else
                    {

                        decimal bal =getexp.FirstOrDefault().TotalBalance;
                        decimal? deb = model.expenserecordForm.Debit;
                        decimal totalbalance = bal - deb.Value;

                        getexp.FirstOrDefault().Description = model.expenserecordForm.Description;
                        getexp.FirstOrDefault().Debit = model.expenserecordForm.Debit;
                        getexp.FirstOrDefault().Credit = 0;
                        getexp.FirstOrDefault().TotalBalance = totalbalance;
                        getexp.FirstOrDefault().ExpensesDate = model.expenserecordForm.ExpensesDate;
                        getexp.FirstOrDefault().ModifiedBy = User.Identity.Name;
                        getexp.FirstOrDefault().ModifiedDate = DateTime.Now;
                        //db.SaveChanges();

                    }
                  
                    TempData["message"] = "The Expenses has been updated successfully.";
                    return RedirectToAction("RecordExp", "Truck", new { Id = getexp.FirstOrDefault().TruckId, ExpTypeId = getexp.FirstOrDefault().ExpensesTypeId, area = "Setup" });
                }
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

        public ActionResult EndExpenses(int Id)
        {
            try
            {
                var getexp = db.ExpensesType.Where(x => x.Id == Id).FirstOrDefault();
                if(getexp !=null)
                {
                    getexp.IsEnded = true;
                    db.SaveChanges();
                    return RedirectToAction("ExpensesList", "Truck", new { Id = getexp.TruckId, exptypeId=getexp.Id, area = "Setup" });
                }
                return RedirectToAction("TruckList", "Truck", new { area = "Setup" });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult ExpensesList(int Id, int exptypeId)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                model.ExpensesList = db.Expenses.Where(x => x.TruckId == Id && x.ExpensesTypeId == exptypeId).ToList();

                model.truck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                model.Expensestype = db.ExpensesType.Where(x => x.Id == exptypeId && x.TruckId == Id).FirstOrDefault();
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;

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


        public ActionResult ViewExpenses(int Id)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                model.truck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                model.ExpensesTypeList = db.ExpensesType.Where(x => x.TruckId == Id).ToList();
                model.ExpensesList = db.Expenses.Where(x => x.TruckId == Id).ToList();
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
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

        public ActionResult FullReport(int Id, int ExpTypeId)
        {
            try
            {
                TruckViewModel model = new TruckViewModel();
                model.truck = db.Truck.Where(x => x.Id == Id).FirstOrDefault();
                model.ExpensesList = db.Expenses.Where(x => x.TruckId == Id && x.ExpensesTypeId==ExpTypeId).ToList();
                model.Expensestype = db.ExpensesType.Where(x => x.Id == ExpTypeId).FirstOrDefault();
                model.FullPhotoPath = Properties.Settings.Default.FullPhotoPath;
               var getprofit = db.Expenses.Where(x => x.TruckId == Id && x.ExpensesTypeId == ExpTypeId).OrderByDescending(x=>x.Id).FirstOrDefault();
                model.Profit = getprofit.TotalBalance;
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
