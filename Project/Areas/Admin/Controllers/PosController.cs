using Project.Areas.Admin.Models;
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
namespace Project.Areas.Admin.Controllers
{
    public class PosController : Controller
    {
        //
        // GET: /Admin/Pos/

        private PROEntities db;
        private ProcessUtility util;
        public PosController()
        {
            db = new PROEntities();
            util = new ProcessUtility();
        }


        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetProduct(int Id)
        {

          

           // string url = "../Content/Backend/SAKDocument/Photo/";
            Product prod = new Product();
            ProductModel model = new ProductModel();
            try
            {
                prod = (from d in db.Product where d.Id == Id select d).FirstOrDefault();
                if (prod != null)
                {                                    
                    model.UnitPrice = prod.ActualPrice;
                    model.UnitDiscountPrice = prod.DiscountPrice;
                    model.ProductImage = prod.Photo1;                  
                    return Json(model, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                TempData["message"] = ex.Message;

            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecordSales(int Id, string TransId)
        {
            try
            {
                PosViewModel model = new PosViewModel();
                var getCustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if (getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.Customer = getCustomer;
                model.Customer.Id = getCustomer.Id;
                model.TransId = TransId;
                model.TransactionList = getCustomer.CustomerTransaction.Where(x => x.TransactionTypeId == Properties.Settings.Default.Credit).ToList();
                var getUncheckPaid = db.ShoppingCart.Where(x => x.Customer.Id == getCustomer.Id && x.HasPaid == false).ToList();
                var balance = getCustomer.CustomerTransaction.OrderByDescending(x => x.Id).FirstOrDefault();
                if(balance != null)
                {
                    model.CurrentBalance = balance.CurrentBalance;
                }
                else
                {
                    model.CurrentBalance = 0;
                }
               
                if (getUncheckPaid.Any())
                {
                   // model.ShoppingCartList = getUncheckPaid;
                    return RedirectToAction("CheckOut", "Pos", new { Id = model.Customer.Id, TransId = getUncheckPaid.FirstOrDefault().TransId });
                    //model.UncheckPaid = true;
                }
                else
                {
                    model.UncheckPaid = false;
                    model.ProductList = (from s in db.Product select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).OrderBy(x => x.Text).ToList();
                    model.ShoppingCartList = (from s in db.ShoppingCart where s.CustomerId == Id && s.TransId == TransId select s).ToList();
                    model.PaymentTypeList = (from p in db.TransactionType where p.ForPos == true select new IntegerSelectListItem { Text = p.Name, Value = p.Id }).OrderBy(x => x.Text).ToList();
                    model.TruckList = (from p in db.Truck where p.IsDeleted == false select new IntegerSelectListItem { Text = p.Name, Value = p.Id }).OrderBy(x => x.Text).ToList();
                    model.paymentForm = new PaymentForm();
                    var check = (from s in db.ShoppingCart where s.TransId == TransId && s.HasPaid==false select s).ToList();
                    if (check.Any())
                    {
                        model.paymentForm.TotalAmount = (from s in db.ShoppingCart where s.CustomerId == Id && s.TransId == TransId select s.TotalPrice).Sum();
                    }
                    else
                    {
                        model.paymentForm.TotalAmount = 0;
                    }
                    if (model.paymentForm.PaymentTypeId == 1)
                    {
                        model.showValue = false;
                    }
                    else
                    {
                        model.showValue = true;
                    }
                }

              
              
                return View("RecordSales", model);
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
        public ActionResult RecordSales(PosViewModel model)
        {
            try
            {
                var GetQuantity = (from p in db.Product where p.Id == model.product.Id select p).FirstOrDefault();
                if (model.Quantity > GetQuantity.Quantity)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "The Quantity you are ordering is higher than what you have in stock.";
                    return RedirectToAction("RecordSales", "Pos", new { Id = model.Customer.Id, TransId = model.TransId });
                }
                else
                {
                    if (model.HasDiscount == true)
                    {
                        int qty = model.Quantity;
                        decimal price = model.product.DiscountPrice;
                        decimal totalPrice = qty * price;
                        ShoppingCart addnew = new ShoppingCart
                        {
                            ProductId = model.product.Id,
                            CustomerId = model.Customer.Id,
                            TransId = model.TransId,
                            Quality = model.Quantity,
                            Price = model.product.DiscountPrice,
                            TotalPrice = totalPrice,
                            HasDiscount = true,
                            HasPaid = false
                        };
                        db.ShoppingCart.AddObject(addnew);
                        db.SaveChanges();

                    }
                    else
                    {
                        int qty = model.Quantity;
                        decimal price = model.product.ActualPrice;
                        decimal totalPrice = qty * price;
                        ShoppingCart addnew = new ShoppingCart
                        {
                            ProductId = model.product.Id,
                            CustomerId = model.Customer.Id,
                            TransId = model.TransId,
                            Quality = model.Quantity,
                            Price = model.product.ActualPrice,
                            TotalPrice = totalPrice,
                            HasDiscount = false,
                            HasPaid = false
                        };
                        db.ShoppingCart.AddObject(addnew);
                        db.SaveChanges();
                    }
                    TempData["message"] = "Item has been added to your cart successfully.";
                    return RedirectToAction("RecordSales", "Pos", new { Id = model.Customer.Id, TransId = model.TransId });
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

        public ActionResult RemoveProduct(int Id, int shoppingId,string TransId)
        {
            try
            {
                var GetShoppingInfo = (from s in db.ShoppingCart where s.Id == shoppingId && s.TransId==TransId select s).FirstOrDefault();
                if (GetShoppingInfo != null)
                {
                    ShoppingCart del = new ShoppingCart
                    {
                    };
                    db.ShoppingCart.DeleteObject(GetShoppingInfo);
                    db.SaveChanges();
                }
                var validate = db.ShoppingCart.Where(x => x.TransId == TransId && x.CustomerId == Id).ToList();
                if(validate.Any())
                {
                    TempData["message"] = "Item has been delete from cart.";
                    return RedirectToAction("RecordSales", new { Id = Id, TransId = GetShoppingInfo.TransId });
                }
                else
                {
                    TempData["message"] = "Item has been delete from cart.";
                    return RedirectToAction("Index", "Customer" ,new {area="Setup" });
                }
               
            }
            catch (Exception ex)
            {
                var GetShoppingInfo = (from s in db.ShoppingCart where s.Id == shoppingId select s).FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "alert-danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                return RedirectToAction("Index", new { Id = GetShoppingInfo.CustomerId, TransId = GetShoppingInfo.TransId });
            }
        }

        public ActionResult CheckOut(int Id, string TransId)
        {
            try
            {
                PosViewModel model = new PosViewModel();
             
                var getCustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if (getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.Customer = getCustomer;
                model.Customer.Id = getCustomer.Id;
                model.TransId = TransId;
                model.paymentForm = new PaymentForm();
                var balance = getCustomer.CustomerTransaction.OrderByDescending(x => x.Id).FirstOrDefault();
                if(balance != null)
                {
                    model.CurrentBalance = balance.CurrentBalance;
                }
                else
                {
                    model.CurrentBalance = 0;
                }
                model.TruckList = (from p in db.Truck where p.IsDeleted == false select new IntegerSelectListItem { Text = p.Name, Value = p.Id }).OrderBy(x => x.Text).ToList();
                model.ProductList = (from s in db.Product select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).OrderBy(x => x.Text).ToList();
                model.ShoppingCartList = (from s in db.ShoppingCart where s.CustomerId == Id && s.TransId == TransId select s).ToList();
                model.PaymentTypeList = (from p in db.TransactionType where p.ForPos==true select new IntegerSelectListItem { Text = p.Name, Value = p.Id }).OrderBy(x => x.Text).ToList();
                model.TransactionList = getCustomer.CustomerTransaction.Where(x=>x.TransactionTypeId==Properties.Settings.Default.Credit).ToList();
                var check = (from s in db.ShoppingCart where s.TransId == TransId && s.HasPaid == false select s).ToList();
                if (check.Any())
                {
                    model.paymentForm.TotalAmount = (from s in db.ShoppingCart where s.CustomerId == Id && s.TransId == TransId select s.TotalPrice).Sum();
                }
                else
                {
                    model.paymentForm.TotalAmount = 0;
                }
                if (model.paymentForm.PaymentTypeId == 1)
                {
                    model.showValue = false;
                }
                else
                {
                    model.showValue = true;
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
        public ActionResult MakePayment(PosViewModel model)
        {
            try
            {
               
                    var getCustomer = db.Customer.Where(x => x.Id == model.Customer.Id).FirstOrDefault();
                    if (getCustomer == null)
                    {
                        TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator.";
                        TempData["messageType"] = "danger";
                        return RedirectToAction("Index");
                    }
                    model.Customer = getCustomer;
                    model.ProductList = (from s in db.Product select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).OrderBy(x => x.Text).ToList();
                    model.ShoppingCartList = (from s in db.ShoppingCart where s.CustomerId == getCustomer.Id && s.TransId == model.TransId select s).ToList();
                    model.PaymentTypeList = (from p in db.TransactionType where p.ForPos == true select new IntegerSelectListItem { Text = p.Name, Value = p.Id }).OrderBy(x => x.Text).ToList();
                    model.TransactionList = getCustomer.CustomerTransaction.Where(x => x.TransactionTypeId == Properties.Settings.Default.Credit).ToList();
                    model.TruckList = (from p in db.Truck where p.IsDeleted ==false select new IntegerSelectListItem { Text = p.Name, Value = p.Id }).OrderBy(x => x.Text).ToList();
                    var check = (from s in db.ShoppingCart where s.TransId == model.TransId && s.HasPaid == false select s).ToList();
                    if (check.Any())
                    {
                        model.paymentForm.TotalAmount = (from s in db.ShoppingCart where s.CustomerId == getCustomer.Id && s.TransId == model.TransId select s.TotalPrice).Sum();
                    }
                    else
                    {
                        model.paymentForm.TotalAmount = 0;
                    }
                    if (model.paymentForm.PaymentTypeId == 1)
                    {
                        model.showValue = false;
                    }
                    else
                    {
                        model.showValue = true;
                    }
                if (ModelState.IsValid)
                {
                    CodeGenerator CodePhoto = new CodeGenerator();

                    if (model.paymentForm.PaymentTypeId==Properties.Settings.Default.SAKAccount)
                    {
                        decimal getAccountBalance = getCustomer.CustomerTransaction.OrderByDescending(x=>x.Id).FirstOrDefault().CurrentBalance;
                        decimal AmountPayable = model.paymentForm.TotalAmount;
                        if(AmountPayable < getAccountBalance)
                        {
                            decimal TotalCurrentBalance = getAccountBalance - AmountPayable ;

                           
                            CustomerTransaction addnew = new CustomerTransaction
                            {
                                CustomerId = getCustomer.Id,
                                TransactionTypeId = Properties.Settings.Default.Debit,
                                Amount = AmountPayable,
                                CurrentBalance = TotalCurrentBalance,
                                Description = model.paymentForm.OrderDescription,
                                TransactionDate = DateTime.Now,
                                TransactionNo = CodePhoto.PadZeroes(6, CodePhoto.GetNextID("TransNo")),
                                CreatedBy = User.Identity.Name,
                                CreatedDate = DateTime.Now,
                                ModifiedBy = User.Identity.Name,
                                ModifiedDate = DateTime.Now,
                                IsDeleted = false,
                                ReportDate = DateTime.Today.ToShortDateString()

                            };
                            db.CustomerTransaction.AddObject(addnew);
                            Payment newpay = new Payment
                            {
                                TransId = model.TransId,
                                TransactionTypeId = model.paymentForm.PaymentTypeId,
                                TotalAmount = AmountPayable,
                                AmountPaid = AmountPayable,
                                Balance = 0,
                                OrderDescription = model.paymentForm.OrderDescription,
                                OrderNo = CodePhoto.PadZeroes(6, CodePhoto.GetNextID("TransNo")),
                                PaymentDate = DateTime.Now,
                                CustomerId = getCustomer.Id,
                                PayDateReport = DateTime.Today.ToShortDateString(),
                                TruckId = model.paymentForm.TruckId,


                            };
                            db.Payment.AddObject(newpay);
                            db.SaveChanges();

                           


                            var getCart = db.ShoppingCart.Where(x => x.CustomerId == getCustomer.Id && x.TransId == model.TransId && x.HasPaid == false).ToList();
                            foreach (var c in getCart)
                            {
                                var product = db.Product.Where(x => x.Id == c.ProductId).FirstOrDefault();

                                int oldqty = product.Quantity;
                                int qty_order = c.Quality;
                                int total_qty_order = oldqty - qty_order;
                                product.Quantity = total_qty_order;

                                var updateitem = db.ShoppingCart.Where(x => x.Id == c.Id).FirstOrDefault();
                                updateitem.HasPaid = true;
                                db.SaveChanges();
                            }
                            TempData["message"] = "Good! Payment has been made successfully.";
                            return RedirectToAction("CustomerOrderHistory", "Customer", new { Id = model.Customer.Id,TransId=model.TransId, area = "Setup" });
                        }
                        else
                        {
                            TempData["messageType"] = "danger";
                            TempData["message"] = "You do not have Insufficient fund on this your account.";
                            return RedirectToAction("CheckOut", "Pos", new { Id = model.Customer.Id, TransId = model.TransId, area = "Admin" });
                        }

                       
                    }
                    if(model.paymentForm.PaymentTypeId==Properties.Settings.Default.Credit)
                    {
                        Payment newpay = new Payment
                        {
                            TransId = model.TransId,
                            TransactionTypeId = model.paymentForm.PaymentTypeId,
                            TotalAmount = model.paymentForm.TotalAmount,
                            AmountPaid = 0,
                            Balance = model.paymentForm.TotalAmount,
                            OrderDescription = model.paymentForm.OrderDescription,
                            OrderNo = CodePhoto.PadZeroes(6, CodePhoto.GetNextID("TransNo")),
                            PaymentDate = DateTime.Now,
                            CustomerId = getCustomer.Id,
                            PayDateReport = DateTime.Today.ToShortDateString(),
                            TruckId = model.paymentForm.TruckId,
                        };
                        db.Payment.AddObject(newpay);
                        db.SaveChanges();
                        var getCart = db.ShoppingCart.Where(x => x.CustomerId == getCustomer.Id && x.TransId == model.TransId && x.HasPaid == false).ToList();
                        foreach (var c in getCart)
                        {
                            var product = db.Product.Where(x => x.Id == c.ProductId).FirstOrDefault();

                            int oldqty = product.Quantity;
                            int qty_order = c.Quality;
                            int total_qty_order = oldqty - qty_order;
                            product.Quantity = total_qty_order;

                            var updateitem = db.ShoppingCart.Where(x => x.Id == c.Id).FirstOrDefault();
                            updateitem.HasPaid = false;
                            db.SaveChanges();
                        }
                        TempData["message"] = "Good! Product(s) has been recorded made successfully on credit.";
                        return RedirectToAction("CustomerOrderHistory", "Customer", new { Id = model.Customer.Id, TransId = model.TransId, area = "Setup" });
                    }
                    if(model.paymentForm.PaymentTypeId== Properties.Settings.Default.Cash || model.paymentForm.PaymentTypeId == 6)
                    {                      
                        Payment newpay = new Payment
                        {
                            TransId = model.TransId,
                            TransactionTypeId = model.paymentForm.PaymentTypeId,
                            TotalAmount = model.paymentForm.TotalAmount,
                            AmountPaid = model.paymentForm.TotalAmount,
                            Balance = 0,
                            OrderDescription = model.paymentForm.OrderDescription,
                            OrderNo = CodePhoto.PadZeroes(6, CodePhoto.GetNextID("TransNo")),
                            PaymentDate = DateTime.Now,
                            CustomerId = getCustomer.Id,
                            PayDateReport = DateTime.Today.ToShortDateString(),
                            TruckId = model.paymentForm.TruckId,
                        };
                        db.Payment.AddObject(newpay);
                        db.SaveChanges();
                        var getCart = db.ShoppingCart.Where(x => x.CustomerId == getCustomer.Id && x.TransId == model.TransId && x.HasPaid == false).ToList();
                        foreach (var c in getCart)
                        {
                            var product = db.Product.Where(x => x.Id == c.ProductId).FirstOrDefault();

                            int oldqty = product.Quantity;
                            int qty_order = c.Quality;
                            int total_qty_order = oldqty - qty_order;
                            product.Quantity = total_qty_order;

                            var updateitem = db.ShoppingCart.Where(x => x.Id == c.Id).FirstOrDefault();
                            updateitem.HasPaid = true;
                            db.SaveChanges();
                        }
                        TempData["message"] = "Good! Payment has been made successfully.";
                        return RedirectToAction("CustomerOrderHistory", "Customer", new { Id = model.Customer.Id, TransId = model.TransId, area = "Setup" });

                    }
                    if(model.paymentForm.PaymentTypeId== Properties.Settings.Default.Installmental)
                    {
                        if (model.paymentForm.AmountPaid > model.paymentForm.TotalAmount)
                        {
                            TempData["message"] = "Error: The amount you are paying is less than or higher than what is to be paid ";
                            TempData["messageType"] = "danger";

                            return RedirectToAction("CheckOut", "Pos", new { Id = model.Customer.Id, TransId = model.TransId, area = "Admin" });
                        }

                        decimal amount_paid = model.paymentForm.TotalAmount - model.paymentForm.AmountPaid;

                        Payment newpay = new Payment
                        {
                            TransId = model.TransId,
                            TransactionTypeId = model.paymentForm.PaymentTypeId,
                            TotalAmount = model.paymentForm.TotalAmount,
                            AmountPaid = model.paymentForm.AmountPaid,
                            Balance = amount_paid,
                            OrderDescription = model.paymentForm.OrderDescription,
                            OrderNo = CodePhoto.PadZeroes(6, CodePhoto.GetNextID("TransNo")),
                            PaymentDate = DateTime.Now,
                            CustomerId = getCustomer.Id,
                            PayDateReport = DateTime.Today.ToShortDateString(),
                            TruckId = model.paymentForm.TruckId,

                        };
                        db.Payment.AddObject(newpay);
                        db.SaveChanges();


                        InstallmentalTransaction newtrans = new InstallmentalTransaction
                        {

                            PaymentId = newpay.Id,
                            TotalAmount = model.paymentForm.TotalAmount,
                            AmountPaid = model.paymentForm.AmountPaid,
                            Balance = amount_paid,
                            PaymentDate = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            PayReportDate = DateTime.Today.ToShortDateString()

                        };
                        db.InstallmentalTransaction.AddObject(newtrans);
                        db.SaveChanges();


                        var getCart = db.ShoppingCart.Where(x => x.CustomerId == getCustomer.Id && x.TransId == model.TransId && x.HasPaid == false).ToList();
                        foreach (var c in getCart)
                        {
                            var product = db.Product.Where(x => x.Id == c.ProductId).FirstOrDefault();

                            int oldqty = product.Quantity;
                            int qty_order = c.Quality;
                            int total_qty_order = oldqty - qty_order;
                            product.Quantity = total_qty_order;

                            var updateitem = db.ShoppingCart.Where(x => x.Id == c.Id).FirstOrDefault();
                            updateitem.HasPaid = true;
                            db.SaveChanges();
                        }
                        TempData["message"] = "Good! Payment has been made successfully.";
                        return RedirectToAction("CustomerOrderHistory", "Customer", new { Id = model.Customer.Id, TransId = model.TransId, area = "Setup" });
                    }
                }
                TempData["message"] = "Ops! Something wrong. Please make sure you select the payment type and enter order description.";
                TempData["messageType"] = "danger";
               // return View(model);
                return RedirectToAction("CheckOut", "Pos", new {Id=model.Customer.Id,TransId=model.TransId, area = "Admin" });
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult InstallmentalPayment(int Id, string TransId)
        {
            try
            {
                PosViewModel model = new PosViewModel();
                var getCustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if (getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                model.Customer = getCustomer;
               
                model.ShoppingCartList = (from s in db.ShoppingCart where s.CustomerId == getCustomer.Id && s.TransId == model.TransId select s).ToList();
                model.TransId = TransId;
                model.paymentForm = new PaymentForm();
                var balance = getCustomer.CustomerTransaction.OrderByDescending(x => x.Id).FirstOrDefault();
                if(balance != null)
                {
                    model.CurrentBalance = balance.CurrentBalance;
                }
                else
                {
                    model.CurrentBalance = 0;
                }
                
                model.PaymentTypeList = (from p in db.TransactionType where p.ForPos == true && p.Id==Properties.Settings.Default.Installmental select new IntegerSelectListItem { Text = p.Name, Value = p.Id }).OrderBy(x => x.Text).ToList();
                model.TransactionList = getCustomer.CustomerTransaction.Where(x => x.TransactionTypeId == Properties.Settings.Default.Credit).ToList();
                model.ShoppingCartList = (from s in db.ShoppingCart where s.CustomerId == Id && s.TransId == TransId select s).ToList();
                var getPayment = getCustomer.Payment.Where(x => x.TransId == TransId).OrderByDescending(x=>x.Id).FirstOrDefault();
                model.payment = getPayment;
                var check = (from s in db.ShoppingCart where s.TransId == model.TransId && s.HasPaid == false select s).ToList();
                if (check.Any())
                {
                    model.paymentForm.TotalAmount = (from s in db.ShoppingCart where s.CustomerId == getCustomer.Id && s.TransId == model.TransId select s.TotalPrice).Sum();
                }
                else
                {
                    model.paymentForm.TotalAmount = 0;
                }
                //if (model.paymentForm.PaymentTypeId == Properties.Settings.Default.Installmental)
                //{
                //    model.showValue = false;
                //}
                //else
                //{
                //    model.showValue = true;
                //}
                return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator";
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult InstallmentalPayment(PosViewModel model)
        {
            try
            {
                var getCustomer = db.Customer.Where(x => x.Id == model.Customer.Id).FirstOrDefault();
                if (getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                model.Customer = getCustomer;
                model.ProductList = (from s in db.Product select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).OrderBy(x => x.Text).ToList();
                model.ShoppingCartList = (from s in db.ShoppingCart where s.CustomerId == getCustomer.Id && s.TransId == model.TransId select s).ToList();
                model.PaymentTypeList = (from p in db.TransactionType where p.ForPos == true select new IntegerSelectListItem { Text = p.Name, Value = p.Id }).OrderBy(x => x.Text).ToList();
                model.TransactionList = getCustomer.CustomerTransaction.Where(x => x.TransactionTypeId == Properties.Settings.Default.Credit).ToList();

              

                if (ModelState.IsValid)
                {

                    var getPayment = getCustomer.Payment.Where(x => x.TransId == model.TransId).OrderByDescending(x => x.Id).FirstOrDefault();
                    model.payment = getPayment;

                   

                    if (model.UseAccount==true)
                    {
                     

                        //Pay with User Account
                        decimal getAccountBalance = getCustomer.CustomerTransaction.OrderByDescending(x => x.Id).FirstOrDefault().CurrentBalance;
                        if(getAccountBalance==0)
                        {
                            TempData["message"] = "You do not have Insufficient fund on this your account.";
                            TempData["messageType"] = "danger";
                            return RedirectToAction("InstallmentalPayment", "Pos", new { Id = getCustomer.Id, TransId = model.TransId, area = "Admin" });
                        }
                        decimal balancetopay =      model.payment.Balance;
                        if (getAccountBalance < balancetopay)
                        {
                            TempData["message"] = "You do not have Insufficient fund on this your account.";
                            TempData["messageType"] = "danger";
                            return RedirectToAction("InstallmentalPayment", "Pos", new { Id = getCustomer.Id, TransId = model.TransId, area = "Admin" });
                        }
                        else
                        {


                            decimal current_bal = getAccountBalance - model.payment.Balance;

                            decimal Totaltopay = getAccountBalance - balancetopay;
                            CodeGenerator CodePhoto = new CodeGenerator();

                            CustomerTransaction addnew = new CustomerTransaction
                            {
                                CustomerId = getCustomer.Id,
                                TransactionTypeId = Properties.Settings.Default.Debit,
                                Amount = Totaltopay,
                                CurrentBalance = current_bal,
                                Description = model.paymentForm.OrderDescription,
                                TransactionDate = DateTime.Now,
                                TransactionNo = CodePhoto.PadZeroes(6, CodePhoto.GetNextID("TransNo")),
                                CreatedBy = User.Identity.Name,
                                CreatedDate = DateTime.Now,
                                ModifiedBy = User.Identity.Name,
                                ModifiedDate = DateTime.Now,
                                IsDeleted = false,
                                ReportDate = DateTime.Today.ToShortDateString()

                            };
                            db.CustomerTransaction.AddObject(addnew);
                            db.SaveChanges();


                            decimal paid = getPayment.AmountPaid + model.payment.Balance;
                            decimal bal = (getPayment.AmountPaid + model.payment.Balance) - getPayment.TotalAmount;

                            InstallmentalTransaction newtrans1 = new InstallmentalTransaction
                            {

                                PaymentId = getPayment.Id,
                                TotalAmount = getPayment.TotalAmount,
                                AmountPaid = model.payment.Balance,
                                Balance = bal,
                                PaymentDate = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                PayReportDate = DateTime.Today.ToShortDateString()

                            };
                            db.InstallmentalTransaction.AddObject(newtrans1);
                            db.SaveChanges();

                            getPayment.AmountPaid = paid;
                            getPayment.Balance = bal;
                            getPayment.OrderDescription = model.paymentForm.OrderDescription;
                            getPayment.PaymentDate = DateTime.Now;
                            db.SaveChanges();


                            TempData["message"] = "Good! Payment has been made successfully.";
                            return RedirectToAction("PaymentHistory", "Pos", new { Id = getCustomer.Id, TransId = model.TransId, area = "Admin" });

                        }

                    }
                    

                    decimal lastpayment = getPayment.AmountPaid;
                    decimal newpayment = model.paymentForm.AmountPaid;
                    decimal totalpayment = lastpayment + newpayment;
                    decimal balance = model.payment.Balance - model.paymentForm.AmountPaid;

                    getPayment.AmountPaid = totalpayment;
                    getPayment.Balance = balance;
                    getPayment.OrderDescription = model.paymentForm.OrderDescription;
                    getPayment.PaymentDate = DateTime.Now;
                   db.SaveChanges();

                    
                    InstallmentalTransaction newtrans = new InstallmentalTransaction
                    {

                       PaymentId = getPayment.Id,
                       TotalAmount = getPayment.TotalAmount,
                       AmountPaid = model.paymentForm.AmountPaid,
                       Balance = balance,
                       PaymentDate = DateTime.Now,
                       CreatedBy = User.Identity.Name,
                       PayReportDate = DateTime.Today.ToShortDateString(),

                    };
                    db.InstallmentalTransaction.AddObject(newtrans);
                    db.SaveChanges();
                    TempData["message"] = "Good! Payment has been made successfully.";
                    return RedirectToAction("PaymentHistory", "Pos", new { Id = getCustomer.Id, TransId=model.TransId, area = "Admin" });
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

        public ActionResult PaymentHistory(int Id, string TransId)
        {
            try
            {
                PosViewModel model = new PosViewModel();
                var getCustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if (getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                model.TransId = TransId;
               
                model.Customer = getCustomer;
                model.Customer.Id = Id;

                var getpayment = db.Payment.Where(x => x.TransId == TransId && x.CustomerId == model.Customer.Id).FirstOrDefault();
                var getHistory = getpayment.InstallmentalTransaction.OrderByDescending(x=>x.Id).ToList();
                model.InstallmentalTransactionlist = getHistory;
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
    }
}
