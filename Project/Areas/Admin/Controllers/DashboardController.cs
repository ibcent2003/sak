using Project.Areas.Admin.Models;
using Project.DAL;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Admin/Dashboard/

        private PROEntities db;
       
        public DashboardController()
        {
            db = new PROEntities();
          
        }

        public ActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel();
            var getcustomers = db.Customer.OrderBy(x => x.Name).ToList();
            model.TotalCustomers = getcustomers.Count();

            var getproducts = db.Product.ToList();
            model.TotalProduct = getproducts.Count();

            var getpayments = db.Payment.ToList();
            model.TotalPayment = getpayments.Count();

            var getemployee = db.Employee.ToList();
            model.TotalEmployee = getemployee.Count();

            var getreorder = db.Product.Where(x => x.ReoderLevel >= x.Quantity).ToList();
            model.Reoder = getreorder.Count();

            var getrawMaterial = db.RawMaterial.Where(x=>x.MaterialTypeId==2).ToList();
            model.TotalRawmaterial = getrawMaterial.Count();

            var gettruck = db.Truck.ToList();
            model.TotalTruck = gettruck.Count();

            var getsales = db.Payment.Select(x=>x.TransactionTypeId).Distinct().ToList();
            model.TotalSalesType = getsales.Count();


            return View(model);
        }

        public ActionResult PaymentList()
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var getpayment = db.Payment.OrderByDescending(x=>x.Id).ToList();
                model.paymentList = getpayment;
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


        public ActionResult PaymentTypeList()
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var getpaytype = db.Payment.Select(x => x.TransactionTypeId).Distinct().ToList();
               
                model.paymenttypeList = getpaytype;
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

        public ActionResult TransactionList(int TransId, DateTime? fromdate, DateTime? todate)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel();
                var getpayment = db.Payment.Where(x=>x.TransactionTypeId==TransId).OrderByDescending(x => x.Id).ToList();
                model.paymentList = getpayment;
                model.documentPath = Properties.Settings.Default.FullPhotoPath;
                model.TransId = TransId;
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
        public ActionResult TransactionList(DashboardViewModel model)
        {
            try
            {
                var endDateExclusive = model.searchForm.ToDate.AddDays(1);
                var getpayment = db.Payment.Where(x => x.PaymentDate >= model.searchForm.FromDate && x.PaymentDate < endDateExclusive && x.TransactionTypeId==model.TransId).OrderByDescending(x => x.Id).ToList();
                model.paymentList = getpayment;
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

    }
}
