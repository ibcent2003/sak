using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project.Areas.Setup.Models;
using Project.DAL;

namespace Project.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        //first list
        public int TotalCustomers { get; set; }
        public int TotalProduct { get; set; }
        public int TotalPayment { get; set; }
        public int TotalEmployee { get; set; }

        public int TransId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }



        //second list
        public int TotalRawmaterial { get; set; }
        public int TotalTruck { get; set; }

        public  int Reoder { get; set; }

        public int TotalSalesType { get; set; }

        public IList<Payment> paymentList { get; set; }

        public IList<Payment> paymenttype { get; set; }
        public List<int> paymenttypeList { get; set; }

        public SearchForm searchForm { get; set; }

        public string documentPath { get; set; }


    }
}