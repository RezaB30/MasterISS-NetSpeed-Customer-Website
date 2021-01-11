using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Supports
{
    public class SupportMessagesViewModel
    {
        public long ID { get; set; }
        public string SupportRequestName { get; set; } //  ex: fatura
        public string SupportRequestSummary { get; set; } // ex: Faturamı ödeyemiyorum
        [Display(ResourceType = typeof(CMS.Localization.Models.Models) , Name = "SupportNo")]
        public string SupportNo { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "SupportDate")]
        public DateTime SupportDate { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "Department")]
        public string Department { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "State")]
        public string State { get; set; }   // complete or processing
        public int StateId { get; set; }
        public IEnumerable<SupportMessageList> SupportMessageList { get; set; }
        public SupportRequestDisplayTypes SupportDisplayType { get; set; }
        public string Message { get; set; }
    }
    public class SupportMessageList
    {
        public string Message { get; set; }
        [UIHint("DateTimeHour")]
        public DateTime MessageDate { get; set; }
        public string SenderName { get; set; } // agent or customer
        public bool IsCustomer { get; set; }
        public IEnumerable<SupportFileList> Files { get; set; }
    }
    public class SupportFileList
    {
        public string FileUrl { get; set; }
        public string FileName { get; set; }
    }
}
