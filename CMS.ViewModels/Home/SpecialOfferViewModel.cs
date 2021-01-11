using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class SpecialOfferViewModel
    {
        public SpecialOfferViewModel()
        {
            SpecialOfferList = Enumerable.Empty<SpecialOfferList>();
        }
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
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "ReferenceNo")]
        public string ReferenceNo { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "StartDate")]
        [UIHint("MonthAndYear")]
        public DateTime StartDate { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "EndDate")]
        [UIHint("MonthAndYear")]
        public DateTime EndDate
        {
            get
            {
                return StartDate.AddMonths(TotalCount);
            }
        }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "UsedCount")]
        public int UsedCount { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "TotalCount")]
        public int TotalCount { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "MissedCount")]
        public int MissedCount { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "RemainingCount")]
        public int RemainingCount { get { return IsCancelled ? 0 : TotalCount - (UsedCount + MissedCount); } }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "ThisPeriod")]
        public bool IsApplicableThisPeriod
        {
            get
            {
                return RemainingCount > 0 && !IsCancelled;
            }
        }

        public bool IsCancelled { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "SubscriberState")]
        [EnumType(typeof(CMS.Localization.Enums.CustomerState), typeof(RadiusR.Localization.Lists.CustomerState))]
        [UIHint("LocalizedList")]
        public short? ReferralSubscriberState { get; set; }
    }
}
