using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Payment
{
    public class ActivateAutomaticPaymentViewModel
    {
        [Required]
        public string CardToken { get; set; }

        [Required]
        public long SubscriptionID { get; set; }

        [Required]
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "PaymentType")]
        [EnumType(typeof(CMS.Localization.Enums.AutoPaymentType), typeof(RadiusR.Localization.Lists.AutoPaymentType))]
        [UIHint("LocalizedList")]
        public short PaymentType { get; set; }
    }
}
