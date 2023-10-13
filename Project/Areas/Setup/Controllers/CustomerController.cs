using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class CustomerController : Controller
    {
        private PROEntities db = new PROEntities();
        public ActionResult Index([DefaultValue(1)] int page, string keywords, [DefaultValue(12)] int pgsize)
        {
            try
            {
                List<Customer> rowsToShow = new List<Customer>();
                int totalRecords = 0;

                if (keywords != null)
                {
                    rowsToShow = db.Customer.Where(s => s.Name.Contains(keywords)).OrderBy(x => x.Name).Skip((page - 1) * pgsize).Take(pgsize).ToList();
                    totalRecords = db.Customer.Where(s => s.Name.Contains(keywords)).Count();
                }
                else
                {
                    rowsToShow = db.Customer.OrderBy(x => x.Name).Skip((page - 1) * pgsize).Take(pgsize).ToList();
                    totalRecords = db.Customer.Count();
                }

                var model = new CustomerViewModel
                {
                    CustomerList = rowsToShow,
                    PagingInfo = new PagingInfo
                    {
                        FirstItem = ((page - 1) * pgsize) + 1,
                        LastItem = page * pgsize,
                        CurrentPage = page,
                        ItemsPerPage = pgsize,
                        TotalItems = totalRecords
                    },
                    CurrentKeywords = keywords,
                    PageSize = pgsize

                };

                
                Random rand = new Random();
                long randnum2 = (long)(rand.NextDouble() * 90000) + 10000;
                model.TransId = randnum2.ToString();
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
        public ActionResult NewCustomer()
        {
            try
            {
                CustomerViewModel model = new CustomerViewModel();               
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
        public ActionResult NewCustomer(CustomerViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var getCustomer = db.Customer.Where(x => x.Name == model.customerForm.Name).ToList();
                    if(getCustomer.Any())
                    {
                        TempData["message"] = "ERROR: The customer name " + model.customerForm.Name + " already exist. Please enter different name.";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    //do insert here
                    Customer addnew = new Customer
                    {
                        Name = model.customerForm.Name,
                        MobileNo = model.customerForm.MobileNo,
                        EmailAddress = model.customerForm.EmailAddress,
                        ContactAddress = model.customerForm.ContactAddress,
                        IsDeleted = model.customerForm.IsDeleted,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    db.Customer.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "<b>" +model.customerForm.Name+ "</b> has been added successfully.";
                    return RedirectToAction("Index");
                }
                TempData["message"] = "There is error submitting customer information. Please sure you enter all filds with * sign.";
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
        public ActionResult EditCustomer(int Id)
        {
            try
            {
                CustomerViewModel model = new CustomerViewModel();
                var getCustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if(getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please start the process of editting a customer.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.customerForm = new CustomerForm();
                model.customerForm.Id = getCustomer.Id;
                model.customerForm.Name = getCustomer.Name;
                model.customerForm.MobileNo = getCustomer.MobileNo;
                model.customerForm.EmailAddress = getCustomer.EmailAddress;
                model.customerForm.ContactAddress = getCustomer.ContactAddress;
                model.customerForm.IsDeleted = getCustomer.IsDeleted;
                model.Customer = getCustomer;
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
        public ActionResult EditCustomer(CustomerViewModel model)
        {
            try
            {
                var getCustomer = db.Customer.Where(x => x.Id == model.customerForm.Id).FirstOrDefault();
                if (getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please start the process of editting a customer.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                if (ModelState.IsValid)
                {
                    getCustomer.Name = model.customerForm.Name;
                    getCustomer.MobileNo = model.customerForm.MobileNo;
                    getCustomer.EmailAddress = model.customerForm.EmailAddress;
                    getCustomer.ContactAddress = model.customerForm.ContactAddress;
                    getCustomer.IsDeleted = model.customerForm.IsDeleted;
                    getCustomer.ModifiedBy = User.Identity.Name;
                    getCustomer.ModifiedDate = DateTime.Now;
                    db.Employee.Context.SaveChanges();
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.customerForm.Name + "</b> has been updated successfully.";
                    return RedirectToAction("Index");
                }
                TempData["message"] = "There is error submitting customer information. Please sure you enter all filds with * sign.";
                TempData["messageType"] = "danger";
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
        public ActionResult CustomerTransaction(int Id)
        {
            try

            { 
                CustomerViewModel model = new CustomerViewModel();
                var getCustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if (getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.Customer = getCustomer;
                model.TransactionList = getCustomer.CustomerTransaction.ToList();
                var balance = getCustomer.CustomerTransaction.OrderByDescending(x => x.Id).FirstOrDefault();
                if(balance !=null)
                {
                    model.CurrentBalance = balance.CurrentBalance;
                }
                else
                {
                    model.CurrentBalance = 0;
                }
               

                model.TransactionTypeList = (from x in this.db.TransactionType.ToList<TransactionType>()where x.Id==1 select new SelectListItem(){Text = x.Name,Value = x.Id.ToString()}).ToList<SelectListItem>();

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
        public ActionResult CustomerTransaction(CustomerViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var getCustomer = db.Customer.Where(x => x.Id == model.Customer.Id).FirstOrDefault();
                    if (null == getCustomer)
                    {
                        TempData["message"] = "Ops! something went wrong while adding a new customer. Please try again or contact the system administrator";
                        TempData["messageType"] = "danger";
                        return RedirectToAction("Index", "Customer", new { area = "Setup" });
                    }
                    CodeGenerator CodeGen = new CodeGenerator();
                    var getBalance = getCustomer.CustomerTransaction.OrderByDescending(x=>x.Id).FirstOrDefault();
                    if(getBalance!= null)
                    {
                        decimal newAmount = model.transactionForm.Amount;
                        decimal totalBalance = getBalance.CurrentBalance + newAmount;
                        CustomerTransaction addnew = new CustomerTransaction
                        {
                            CustomerId = getCustomer.Id,
                            TransactionTypeId = model.transactionForm.TransactionTypeId,
                            Amount = model.transactionForm.Amount,
                            Description = model.transactionForm.Description,
                            CurrentBalance = totalBalance,
                            TransactionDate = DateTime.Now,
                            TransactionNo = CodeGen.PadZeroes(6, CodeGen.GetNextID("TransNo")),
                            CreatedBy = User.Identity.Name,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                            ModifiedDate = DateTime.Now,
                            IsDeleted = false,
                            ReportDate = DateTime.Today.ToShortDateString()
                        };
                        db.CustomerTransaction.AddObject(addnew);
                        db.SaveChanges();
                        TempData["message"] = "Transaction has been recorded successfully.";
                        return RedirectToAction("CustomerTransaction", "Customer", new { Id = getCustomer.Id, area = "Setup" });
                    }
                    else
                    {
                        CustomerTransaction addnew = new CustomerTransaction
                        {
                            CustomerId = getCustomer.Id,
                            TransactionTypeId = model.transactionForm.TransactionTypeId,
                            Amount = model.transactionForm.Amount,
                            Description = model.transactionForm.Description,
                            CurrentBalance = model.transactionForm.Amount,
                            TransactionDate = DateTime.Now,
                            TransactionNo = CodeGen.PadZeroes(6, CodeGen.GetNextID("TransNo")),
                            CreatedBy = User.Identity.Name,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                            ModifiedDate = DateTime.Now,
                            IsDeleted = false,
                            ReportDate = DateTime.Today.ToShortDateString()
                        };
                        db.CustomerTransaction.AddObject(addnew);
                        db.SaveChanges();
                        TempData["message"] = "Transaction has been recorded successfully.";
                        return RedirectToAction("CustomerTransaction", "Customer", new { Id = getCustomer.Id, area = "Setup" });
                    }
                    
                   

                    //ToDO: Add more information to customer transaction

                    
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

        public ActionResult EditCustomerTransaction(int Id, int TransId)
        {
            try
            {
                CustomerViewModel model = new CustomerViewModel();
                var getcustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if(null == getcustomer)
                {
                    TempData["message"] = "Ops! something went wrong while update a customer. Please try again or contact the system administrator";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Customer", new { area = "Setup" });
                }
                var getTransaction = getcustomer.CustomerTransaction.Where(x => x.Id == TransId).FirstOrDefault();
                model.Customer = getcustomer;
                model.transactionForm = new CustomerTransactionForm();
                model.transactionForm.Amount = getTransaction.Amount;
                model.transactionForm.Description = getTransaction.Description;
                model.transactionForm.Id = TransId;
                model.transactionForm.TransactionTypeId = getTransaction.TransactionTypeId;
                model.TransactionTypeList = (from x in this.db.TransactionType.ToList<TransactionType>()where x.Id==1 select new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList<SelectListItem>();
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
        public ActionResult EditCustomerTransaction(CustomerViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var getCustomer = db.Customer.Where(x => x.Id == model.Customer.Id).FirstOrDefault();
                    if (null == getCustomer)
                    {
                        TempData["message"] = "Ops! something went wrong while updaing transaction. Please try again or contact the system administrator";
                        TempData["messageType"] = "danger";
                        return RedirectToAction("Index", "Customer", new { area = "Setup" });
                    }
                    var getTransaction = getCustomer.CustomerTransaction.Where(x => x.Id == model.transactionForm.Id).FirstOrDefault();
                    if(null == getTransaction)
                    {
                        TempData["message"] = "Ops! something went wrong while updaing transaction. Please try again or contact the system administrator";
                        TempData["messageType"] = "danger";
                        return RedirectToAction("CustomerTransaction", "Customer", new {Id= getCustomer.Id, area = "Setup" });
                    }
                    getTransaction.Description = model.transactionForm.Description;
                    getTransaction.IsDeleted = model.transactionForm.IsDeleted;
                    getTransaction.ModifiedBy = User.Identity.Name;
                    getTransaction.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "Transaction has been updated successfully.";               
                    return RedirectToAction("CustomerTransaction", "Customer", new { Id = getCustomer.Id, area = "Setup" });
                }
                TempData["message"] = "Ops! something went wrong. Please enter all fields";
                TempData["messageType"] = "danger";
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

        public ActionResult PrintCustomerTransaction(int Id, int TransId)
        {
            try
            {
                CustomerViewModel model = new CustomerViewModel();
                if (Id == 0)
                {
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product Object is empty. Unlikely error."));
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }


                var getcustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if (null == getcustomer)
                {
                    TempData["message"] = "Ops! something went wrong while update a customer. Please try again or contact the system administrator";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Customer", new { area = "Setup" });
                }
                var getTransaction = getcustomer.CustomerTransaction.Where(x => x.Id == TransId).ToList();
                model.Customer = getcustomer;

                model.TransactionList = getTransaction;
                var getProduct = db.Product.Where(X => X.Id == Id).ToList();
               
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

        public ActionResult SearchCustomerTransaction(int Id)
        {
            try
            {
                CustomerViewModel model = new CustomerViewModel();
                var getCustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if (getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.Customer = getCustomer;
                var balance = getCustomer.CustomerTransaction.OrderByDescending(x => x.Id).FirstOrDefault();
                model.CurrentBalance = balance.CurrentBalance;
                model.TransactionList = getCustomer.CustomerTransaction.ToList();
                model.TransactionTypeList = (from x in this.db.TransactionType.ToList<TransactionType>() select new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList<SelectListItem>();
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

        public ActionResult TransactionHistory(int Id)
        {
            try
            {
                CustomerViewModel model = new CustomerViewModel();
                var getCustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if (getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator.";
                  //  Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    //TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                model.Customer = getCustomer;
                model.payment = getCustomer.Payment.OrderByDescending(x=>x.Id).ToList();
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

        public ActionResult CustomerOrderHistory(int Id, string TransId)
        {
            try
            {
                CustomerViewModel model = new CustomerViewModel();
                var getCustomer = db.Customer.Where(x => x.Id == Id).FirstOrDefault();
                if (getCustomer == null)
                {
                    TempData["message"] = "Ops! Something wrong. Please try again later or contact the system administrator.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.Customer = getCustomer;
                model.documentPath = Properties.Settings.Default.FullPhotoPath;
                var getOrderHistory = getCustomer.ShoppingCart.Where(x => x.TransId == TransId).ToList();
                model.cart = getOrderHistory;
                model.TransId = TransId;
                model.payment = getCustomer.Payment.Where(x=>x.TransId==TransId).OrderByDescending(x => x.Id).ToList();
                var getpay = db.Payment.Where(x => x.TransId == TransId).FirstOrDefault();
                if(getpay.TransactionTypeId==Properties.Settings.Default.Installmental)
                {
                    model.Isinstallment = true;
                }
                else
                {
                    model.Isinstallment = false;
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
    }
}
