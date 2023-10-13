using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class RawmaterialController : Controller
    {
        private PROEntities db = new PROEntities();

        public ActionResult Index()
        {
            try
            {
                RawMaterialViewModel model = new RawMaterialViewModel();
                model.rawMaterials = db.RawMaterial.ToList();                
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

        public ActionResult NewMaterial()
        {
            try
            {
                RawMaterialViewModel model = new RawMaterialViewModel();
                model.MaterialTypeList = (from s in this.db.MaterialType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
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
        public ActionResult NewMaterial(RawMaterialViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var validate = db.RawMaterial.Where(x => x.Name == model.RawMaterialform.Name).ToList();
                    if (validate.Any())
                    {
                        model.MaterialTypeList = (from s in this.db.MaterialType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                        TempData["message"] = "ERROR: The material type " + model.RawMaterialform.Name + " already exist. Please enter different name.";
                        TempData["messageType"] = "danger";
                        return View(model);
                    }
                    else
                    {
                        RawMaterial addnew = new RawMaterial
                        {
                            Name = model.RawMaterialform.Name,
                            MaterialTypeId = model.RawMaterialform.MaterialTypeId,
                            Price = model.RawMaterialform.Price,
                            IsDeleted = model.RawMaterialform.IsDeleted,
                            ModifiedBy = User.Identity.Name,
                            ModifiedDate = DateTime.Now
                        };
                        db.RawMaterial.AddObject(addnew);
                        db.SaveChanges();
                        TempData["message"] = "<b>"+model.RawMaterialform.Name+ "</b> Type has been added successful";
                        return RedirectToAction("Index");

                    }

                }
                model.MaterialTypeList = (from s in this.db.MaterialType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                TempData["message"] = "There is Error while adding material";
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

        public ActionResult EditMaterial(int Id)
        {
            try
            {
                RawMaterialViewModel model = new RawMaterialViewModel();
                model.MaterialTypeList = (from s in this.db.MaterialType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                var getMaterial = db.RawMaterial.Where(x => x.Id == Id).FirstOrDefault();
                if(getMaterial == null)
                {
                    TempData["message"] = "There is Error while editing material. Please please entry again.";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.RawMaterialform = new RawMaterialForm();
                model.RawMaterialform.Id = Id;
                model.RawMaterialform.Name = getMaterial.Name;
                model.RawMaterialform.Price = getMaterial.Price;
                model.RawMaterialform.MaterialTypeId = getMaterial.MaterialTypeId;
                model.RawMaterialform.IsDeleted = getMaterial.IsDeleted;
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
        public ActionResult EditMaterial(RawMaterialViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.MaterialTypeList = (from s in this.db.MaterialType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                    var getMaterial = db.RawMaterial.Where(x => x.Id == model.RawMaterialform.Id).FirstOrDefault();
                    getMaterial.Name = model.RawMaterialform.Name;
                    getMaterial.Price = model.RawMaterialform.Price;
                    getMaterial.IsDeleted = model.RawMaterialform.IsDeleted;
                    getMaterial.ModifiedBy = User.Identity.Name;
                    getMaterial.ModifiedDate = DateTime.Now;
                    getMaterial.MaterialTypeId = model.RawMaterialform.MaterialTypeId;
                    db.SaveChanges();
                    TempData["message"] = "<b>" +model.RawMaterialform.Name+ "</b> has been updated successful";
                    return RedirectToAction("Index");
                }
                model.MaterialTypeList = (from s in this.db.MaterialType where s.IsDeleted == false select new IntegerSelectListItem() { Text = s.Name, Value = s.Id }).ToList<IntegerSelectListItem>();
                TempData["message"] = "There is Error while adding employee type";
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

    }
}
