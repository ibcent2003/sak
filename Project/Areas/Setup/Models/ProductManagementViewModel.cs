using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class ProductManagementViewModel
    {
        public List<Product> ProductList { get; set; }
        public List<ReorderHistory> ReorderList { get; set; }
        public ProductForm Productform { get; set; }
        public Product product { get; set; }
        public string documentPath { get; set; }
        public string p1 { get; set; }
        public string p2 { get; set; }
        public string p3 { get; set; }
        public List<IntegerSelectListItem> ProductTypeList { get; set; }

        public bool HasSearch { get; set; }

        public SearchForm searchForm { get; set; }
    }

    public class SearchForm
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class ProductForm
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "Please enter The Product Name")]
        public string Name { get; set; }

        [Display(Name = "Product Description")]
        [Required(ErrorMessage = "Please enter The Product Description")]
        public string Description { get; set; }

        [Display(Name = "Acutal Price")]
        [Required(ErrorMessage = "Please enter Actual price")]
        public decimal AcutalPrice { get; set; }

        [Display(Name = "Discount Price")]
        [Required(ErrorMessage = "Please enter Discount price")]
        public decimal DiscountPrice { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Please enter Quantity")]
        public int Quantity { get; set; }


        
        
        public int NewQuantity { get; set; }


        [Display(Name = "Re-order level")]
        [Required(ErrorMessage = "Please enter Re-order Level")]
        public int ReorderLevel { get; set; }

        [Display(Name = "Photo 1")]       
        public HttpPostedFileBase Photo1 { get; set; }

        [Display(Name = "Photo 2")]
        public HttpPostedFileBase Photo2 { get; set; }
               
        [Display(Name = "Product Type")]
        [Required(ErrorMessage = "Please select a Product Type ")]
        public int RawMaterialId { get; set; }

        public bool IsDeleted { get; set; }
       
    }
}