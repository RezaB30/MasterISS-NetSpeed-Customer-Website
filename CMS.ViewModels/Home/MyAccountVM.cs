using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class MyAccountVM
    {
        public string FullName { get; set; }
        public string SubscriberNo { get; set; }
        public string XDSLNo { get; set; }
        public string ModemUsername { get; set; }
        public string ModemPassword { get; set; }
        public string StaticIP { get; set; }
        public SubscriptionStateTypes SubscriptionStateTypes { get; set; }
        public bool AutomaticPaymentInstruction { get; set; }
        public string LineAddress { get; set; }
        public int PassedInvoice { get; set; }
        public string SpecialOfferReferenceCode { get; set; }
        public bool ChooseSMS { get; set; }
        public bool ChooseMail { get; set; }
        public string ContactPhoneNo { get; set; }
        public string Mail { get; set; }
        public string CustomerServicePassword { get; set; }

    }
    public enum SubscriptionStateTypes
    {
        Active = 1,
        Cancelled = 2,
        Freeze = 3
    }
}
