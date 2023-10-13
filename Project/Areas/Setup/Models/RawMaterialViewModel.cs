using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class RawMaterialViewModel
    {     
        public List<RawMaterial> rawMaterials { get; set; }
        public RawMaterialForm RawMaterialform { get; set; }
        public List<IntegerSelectListItem> MaterialTypeList { get; set; }
    }
    public class RawMaterialForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select the material type")]
        public int MaterialTypeId { get; set; }
        public bool IsDeleted { get; set; }
    }
}