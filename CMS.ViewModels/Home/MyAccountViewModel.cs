using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class MyAccountViewModel
    {
        [Display(Name = "FullName", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string FullName { get; set; }
        [Display(Name = "SubscriberNo", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string SubscriberNo { get; set; }
        [Display(Name = "XDSLNo", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string XDSLNo { get; set; }
        [Display(Name = "ModemUsername", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string ModemUsername { get; set; }
        [Display(Name = "ModemPassword", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string ModemPassword { get; set; }
        [Display(Name = "StaticIP", ResourceType = typeof(CMS.Localization.Models.Models))]
        [UIHint("TextWithPlaceholder")]
        public string StaticIP { get; set; }
        [Display(Name = "SubscriptionLineState", ResourceType = typeof(CMS.Localization.Models.Models))]
        [UIHint("TextWithPlaceholder")]
        public string SubscriptionState { get; set; }
        [Display(Name = "AutomaticPaymentInstruction", ResourceType = typeof(CMS.Localization.Models.Models))]
        public bool AutomaticPaymentInstruction { get; set; }
        [Display(Name = "LineAddress", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string LineAddress { get; set; }
        [Display(Name = "PassedInvoice", ResourceType = typeof(CMS.Localization.Models.Models))]
        public int? PassedInvoice { get; set; }
        [Display(Name = "SpecialOfferReferenceCode", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string SpecialOfferReferenceCode { get; set; }
        [Display(Name = "SMS", ResourceType = typeof(CMS.Localization.Models.Models))]

        public bool ChooseSMS { get; set; }
        [Display(Name = "Email", ResourceType = typeof(CMS.Localization.Models.Models))]
        public bool ChooseMail { get; set; }
        [Display(Name = "ContactPhoneNo", ResourceType = typeof(CMS.Localization.Models.Models))]
        [StringLength(11, MinimumLength = 10, ErrorMessageResourceName = "NotValid", ErrorMessageResourceType = typeof(CMS.Localization.Errors))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        public string ContactPhoneNo { get; set; }
        [Display(Name = "Email", ResourceType = typeof(CMS.Localization.Models.Models))]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessageResourceName = "NotValid", ErrorMessageResourceType = typeof(CMS.Localization.Errors))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        [StringLength(50, ErrorMessageResourceName = "NotValid", ErrorMessageResourceType = typeof(CMS.Localization.Errors))]
        public string Mail { get; set; }
        [Display(Name = "CustomerServicePassword", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string CustomerServicePassword { get; set; }
    }
}
