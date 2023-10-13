using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Models 
{
    public class CustomerViewModel : PageInfoModel
{
        public Customer Customer { get; set; }
        public IList<Customer> CustomerList { get; set; }
        public string documentPath { get; set; }
        public IList<Payment> payment { get; set; }
        public IList<ShoppingCart> cart { get; set; }
        public List<CustomerTransaction> TransactionList { get; set; }
        public CustomerForm customerForm { get; set; }
        public CustomerTransactionForm transactionForm { get; set; }
        public string TransId { get; set; }
        public decimal CurrentBalance { get; set; }
        public List<SelectListItem> TransactionTypeList {get;set;}
        public Payment paymentType { get; set; }
        public bool Isinstallment { get; set; }
    }
    public class CustomerForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter customer name")]        
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter customer mobile no")]
        public string MobileNo { get; set; }

        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter customer contact address")]
        public string ContactAddress { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CustomerTransactionForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please transaction Type")]
        public int TransactionTypeId { get; set; }

        [Required(ErrorMessage = "Please enter amount")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Please enter Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter transaction date")]
        public DateTime TransactionDate { get; set; }
        
        public bool IsDeleted { get; set; }
      
    }
}