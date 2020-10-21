using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class SpecialOfferVM
    {
        public string ReferenceNo { get; set; }
        public IEnumerable<SpecialOfferList> SpecialOfferList { get; set; }
        public int TotalEarnDiscount { get; set; }
        public int TotalUseDiscount { get; set; }
        public int TotalLoseDiscount { get; set; }
        public int TotalRemainingDiscount { get; set; }
        public int TotalThisPeriod { get; set; }
    }
    public class SpecialOfferList
    {
        [Display(ResourceType = typeof(CMS.Localization.Models.Models) , Name = "ReferenceNo")]
        public string ReferenceNo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "BeginDate")]
        public DateTime BeginDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "EndDate")]
        public DateTime EndDate { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "EarnDiscount")]
        public int EarnDiscount { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "UseDiscount")]
        public int UseDiscount { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "LoseDiscount")]
        public int LoseDiscount { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "RemainingDiscount")]
        public int RemainingDiscount { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "ThisPeriodDiscount")]
        public int ThisPeriodDiscount { get; set; }
        public SubscriberStateTypes SubscriberState { get; set; }
    }
    public enum SubscriberStateTypes
    {
        Active = 1,
        Passive = 2
    }
}
