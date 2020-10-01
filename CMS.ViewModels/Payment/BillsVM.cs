using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Payment
{
    public class BillsVM
    {
        public long BillNo { get; set; }
        public decimal BillAmount { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime IssueDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DueDate { get; set; }
        public BillStatusTypes BillStatusTypes { get; set; }
        public int InvoicingPeriod { get; set; }    // faturalama dönemi
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime BeginInvoiceDate { get; set; }  // cari fatura dönemi
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime EndInvoiceDate { get; set; }
    }
    public enum BillStatusTypes
    {
        Paid = 1,
        Unpaid = 2
    }
}
