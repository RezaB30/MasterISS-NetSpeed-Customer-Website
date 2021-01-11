using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Payment
{
    public class PaymentsAndBillsViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "ServiceName")]
        public string ServiceName { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "IssueDate")]
        public DateTime BillDate { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "LastPaymentDate")]
        public DateTime LastPaymentDate { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "Total")]
        [UIHint("Currency")]
        public string Total { get; set; }

        //public BillState Status { get; set; }
        public short Status { get; set; }

        public bool CanBePaid { get; set; }

        public bool HasEArchiveBill { get; set; }
    }
}
