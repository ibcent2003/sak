using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Admin.Models
{
    public class PosViewModel
    {
        public Customer Customer { get; set; }
        public Product product { get; set; }
        public Payment payment { get; set; }
        public IList<InstallmentalTransaction> InstallmentalTransactionlist { get; set; }
        public List<CustomerTransaction> TransactionList { get; set; }
        public List<IntegerSelectListItem> ProductList { get; set; }      
        public List<IntegerSelectListItem> PaymentTypeList { get; set; }
        public List<IntegerSelectListItem> TruckList { get; set; }
        public decimal CurrentBalance { get; set; }
        public IList<ShoppingCart> ShoppingCartList { get; set; }
        public ShoppingCart Shoppingcart { get; set; }

        public bool UncheckPaid { get; set; }

        public bool UseAccount { get; set; }

        public decimal? TotalPayable { get; set; }
        public string TransId { get; set; }
        public int Quantity { get; set; }
        public bool HasDiscount { get; set; }
        public bool showValue { get; set; }
        public PaymentForm paymentForm { get; set; }
        

       // public string OrderDescription { get; set; }




    }

    public class PaymentForm
    {
       

        [Display(Name = "Payment Type")]
        [Required(ErrorMessage = "Please select the payment type")]
        public int PaymentTypeId { get; set; }
       
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }

        [Display(Name = "Order Description")]
        [Required(ErrorMessage = "Please enter the order description.")]
        public string OrderDescription { get; set; }

        [Display(Name = "Truck")]
        [Required(ErrorMessage = "Please select truck")]
        public int TruckId { get; set; }




    }
}