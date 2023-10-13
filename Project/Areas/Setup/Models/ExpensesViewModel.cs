using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class ExpensesViewModel
    {
        public List<InhouseExpensive> ExpensesList { get; set; }
        public ExpenseForm Expenseform { get; set; }
    }

    public class ExpenseForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter name")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter Amount")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        [Required(ErrorMessage = "Please enter Date")]
        [Display(Name = "ExpensiveDate")]
        public DateTime ExpensiveDate { get; set; }

    }

}