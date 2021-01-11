using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Layout
{
    public class BillsLayoutViewModel
    {
        public long BillCount { get; set; }
        [Display(Name = "TotalAmount", ResourceType = typeof(CMS.Localization.Common))]
        [UIHint("Currency")]
        public string TotalAmount { get; set; }
        public IEnumerable<BillList> Bills { get; set; }
        public class BillList
        {
            [Display(Name = "DatedInvoice", ResourceType = typeof(CMS.Localization.Common))]
            public DateTime? IssueDate { get; set; }
            [Display(Name = "Amount", ResourceType = typeof(CMS.Localization.Common))]
            [UIHint("Currency")]
            public string Amount { get; set; }
        }
    }
}
