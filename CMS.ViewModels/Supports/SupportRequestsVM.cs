using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Supports
{
    public class SupportRequestsVM
    {
        public string SupportRequestName { get; set; } //  ex: fatura
        public string SupportRequestSummary { get; set; } // ex: Faturamı ödeyemiyorum
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "SupportNo")]
        public long SupportNo { get; set; } // can be id
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "Date")]
        public DateTime Date { get; set; } // dd.MM.yyyy
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "Department")]
        public string Department { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "State")]
        public string State { get; set; }
        public SupportState SupportState { get; set; }
        public string Note { get; set; }
        public ChangeType? ChangeType { get; set; }
        public string CompletedDate { get; set; }
    }
}
