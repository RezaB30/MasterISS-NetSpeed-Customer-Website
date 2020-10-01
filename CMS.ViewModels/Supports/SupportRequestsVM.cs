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
        public long SupportNo { get; set; } // can be id
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime Date { get; set; } // dd.MM.yyyy
        public string Department { get; set; }
        public string State { get; set; }
        public SupportState SupportState { get; set; }
        public string Note { get; set; }
        public ChangeType? ChangeType { get; set; }
        public string CompletedDate { get; set; }
    }    
}
