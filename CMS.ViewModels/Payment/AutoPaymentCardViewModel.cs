using CMS.ViewModels.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Payment
{
    public class AutoPaymentCardViewModel
    {
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "CardholderName")]
        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^([\p{L}|\.]+\s[\p{L}|\.]+)+$", ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "NotValid")]
        [MaxLength(150, ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "MaxLength")]
        public string CardholderName { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "CardNumber")]
        [Required(ErrorMessage = "*")]
        [CardNumber(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "NotValid")]
        public string CardNo { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "ExpirationDateMonth")]
        [Required(ErrorMessage = "*")]
        [CardExpirationMonth(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "NotValid")]
        public string ExpirationMonth { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "ExpirationDateYear")]
        [Required(ErrorMessage = "*")]
        [CardExpirationYear(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "NotValid")]
        public string ExpirationYear { get; set; }
    }
}
