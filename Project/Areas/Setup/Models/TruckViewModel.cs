using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Project.DAL;
using Project.Models;

namespace Project.Areas.Setup.Models
{
    public class TruckViewModel
    {
        public string UploadedPhoto { get; set; }
        public string FullPhotoPath { get; set; }
        public List<Truck> TruckList { get; set; }
        public decimal Profit { get; set; }
        public decimal Dept { get; set; }

        public List<ExpensesType> ExpensesTypeList { get; set; }

        public List<Expenses> ExpensesList { get; set; }
        public TruckExpense TruckExpense { get; set; }
        public Truck truck { get; set; }

        public ExpensesType Expensestype { get; set; }
        public Expenses expenses { get; set; }
        public List<IntegerSelectListItem> MaterialList { get; set; }
        public List<RawMaterial> AvailableMaterial { get; set; }
        public List<TruckExpense> TruckExpenseList { get; set; }
        public int MaterialId { get; set; }
        public List<IntegerSelectListItem> DriverList { get; set; }
        public List<Employee> AvailableEmployee { get; set; }
        public int EmployeeId { get; set; }
        public int DriverId { get; set; }
        public List<RawMaterial> TruckMaterial { get; set; }
        public List<Employee> EmployeeList { get; set; }
        public TruckForm Truckform { get; set; }
        public TruckExpenseForm TruckExpenseform { get; set; }
        public SearhcForm searhcForm { get; set; }
        public ExpenesTypeForm ExpenesTypeform { get; set; }

        public ExpensesRecordForm expenserecordForm { get; set; }
    }
    public class TruckForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter name")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter number")]
        [Display(Name = "Number")]
        public string Number { get; set; }
        [Display(Name = "Photo")]
        public HttpPostedFileBase Photo { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class TruckExpenseForm
    {
        public int Id { get; set; }

        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter Description")]
        [Display(Name = "Description")]

        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Please enter amount")]
        [Display(Name = "Amount")]

        public DateTime ExpenseDate { get; set; }
        [Required(ErrorMessage = "Please enter Date")]
        [Display(Name = "ExpenseDate")]

        public HttpPostedFileBase Receipt { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ExpenesTypeForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter Expenses Date and Month")]
        [Display(Name = "Exp Date & Month")]
        public string ExpMonth { get; set; }

        [Required(ErrorMessage = "Please enter Opening Balance")]
        [Display(Name = "Opening Balance")]
        public decimal OpeningBalance { get; set; }

        public int TruckId { get; set; }
        
        [Display(Name = "Has Ended")]
        public bool HasEnded { get; set; }

        [Display(Name = "Has Ended")]
        public bool IsLoan { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class SearhcForm
    {
    
        [Required(ErrorMessage = "Please enter From date")]        
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "Please enter To date")]
        public DateTime ToDate { get; set; }

    }


    public class ExpensesRecordForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter Expenses Description")]
        [Display(Name = "Expenses Description")]
        public string Description { get; set; }

        [Display(Name = "Debit")]
        public decimal? Debit { get; set; } = 0;

        [Display(Name = "Credit")]
        public decimal? Credit { get; set; } = 0;

        [Required(ErrorMessage = "Please enter Total Balance")]
        [Display(Name = "Total Balance")]
        public decimal TotalBalance { get; set; }

        [Required(ErrorMessage = "Please enter date")]
        [Display(Name = "Expenses Date")]
        public DateTime ExpensesDate { get; set; }
    }
}