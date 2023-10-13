using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Areas.Admin.Models
{
    public class ProductModel
    {
        public decimal UnitPrice { get; set; }
        public string ProductImage { get; set; }
        public decimal UnitDiscountPrice { get; set; }
    }
}