using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class ExpensesController : Controller
    {
        private PROEntities db = new PROEntities();
        //
        // GET: /Setup/Expenses/

        public ActionResult Index()
        {
            try
            {
                ExpensesViewModel model = new ExpensesViewModel();
                model.ExpensesList = db.InhouseExpensive.ToList();               
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


        public ActionResult NewExpense()
        {
            try
            {
                ExpensesViewModel model = new ExpensesViewModel();
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
        public ActionResult NewExpense(ExpensesViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    InhouseExpensive addnew = new InhouseExpensive
                    {
                        Name = model.Expenseform.Name,
                        Description = model.Expenseform.Description,
                        Amount = model.Expenseform.Amount,
                        ExpensiveDate = model.Expenseform.ExpenseDate,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                       
                    };
                    db.InhouseExpensive.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.Expenseform.Name + "</b> Type has been added successful";
                    return RedirectToAction("Index");
                }
                TempData["message"] = "There is Error while adding Expense";
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

        public ActionResult EditExpense(int Id)
        {
            try
            {
                ExpensesViewModel model = new ExpensesViewModel();
                var getExpenses = db.InhouseExpensive.Where(x => x.Id == Id).FirstOrDefault();
                if(getExpenses == null)
                {
                    TempData["message"] = "There is Error while editing expense. Please please entry again.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.Expenseform = new ExpenseForm();
                model.Expenseform.Id = getExpenses.Id;
                model.Expenseform.Name = getExpenses.Name;
                model.Expenseform.Description = getExpenses.Description;
                model.Expenseform.Amount = getExpenses.Amount;
                model.Expenseform.ExpenseDate = getExpenses.ExpensiveDate;
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
        public ActionResult EditExpense(ExpensesViewModel model)
        {
            try
            {
                var getExp = db.InhouseExpensive.Where(x => x.Id == model.Expenseform.Id).FirstOrDefault();
                if(getExp ==null)
                {
                    TempData["message"] = "There is Error while editing expense. Please please entry again.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                getExp.Name = model.Expenseform.Name;
                getExp.Description = model.Expenseform.Description;
                getExp.Amount = model.Expenseform.Amount;
                getExp.ExpensiveDate = model.Expenseform.ExpenseDate;
                getExp.ModifiedBy = User.Identity.Name;
                getExp.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                TempData["message"] = "<b>" + model.Expenseform.Name + "</b> has been updated successful";
                return RedirectToAction("Index");

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
