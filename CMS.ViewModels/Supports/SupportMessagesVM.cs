using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Supports
{
    public class SupportMessagesVM
    {
        public string SupportRequestName { get; set; } //  ex: fatura
        public string SupportRequestSummary { get; set; } // ex: Faturamı ödeyemiyorum
        public string AgentName { get; set; }
        public int AgentID { get; set; } // ??
        public string CustomerFullName { get; set; }
        public long SupportNo { get; set; }
        public ChangeType? ChangeType { get; set; }
        public SupportState SupportState { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime SupportDate { get; set; }
        public string Department { get; set; }
        public string State { get; set; }   // complete or processing
        public IEnumerable<SupportMessageList> SupportMessageList { get; set; }
    }
    public class SupportMessageList
    {
        public string Message { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime MessageDate { get; set; }
        public SupportMessageSenderType SenderType { get; set; }
        public SupportState SupportState { get; set; }
        public string State { get; set; }   // mesajın atıldığı andaki durumu ele alınacak
        public string SenderName { get; set; } // agent or customer
        public IEnumerable<SupportFileList> Files { get; set; }
    }
    public class SupportFileList
    {
        public string FileUrl { get; set; }
        public string FileName { get; set; }
    }
}
