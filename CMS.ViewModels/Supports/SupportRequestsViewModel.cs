using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Supports
{
    public class SupportRequestsViewModel
    {
        public long SupportId { get; set; }
        public string SupportRequestType { get; set; } //  ex: fatura
        public string SupportRequestSubType { get; set; } // ex: Faturamı ödeyemiyorum
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "SupportNo")]
        public string SupportNo { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "Date")]
        public DateTime Date { get; set; } // dd.MM.yyyy
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "Department")]
        public string Department { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "State")]
        public string State { get; set; }
        public short StateId { get; set; }
        public string Note { get; set; }
        public ChangeType? ChangeType { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "ApprovalDate")]
        public DateTime? ApprovalDate { get; set; }
        public SupportRequestDisplayTypes SupportDisplayType { get; set; }
    }
    public class SupportRequestsAdditional
    {
        public string LastMessage { get; set; }
        public bool IsCustomer { get; set; }
        public int SupportDisplayType { get; set; }
    }
}
