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
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "BillNo")]
        public long BillNo { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "BillAmount")]
        public decimal BillAmount { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "IssueDate")]
        public DateTime IssueDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "DueDate")]
        public DateTime DueDate { get; set; }
        public BillStatusTypes BillStatusTypes { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "InvoicingPeriod")]
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
