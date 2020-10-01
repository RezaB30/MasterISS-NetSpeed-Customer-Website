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
        public string ReferenceNo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime BeginDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime EndDate { get; set; }
        public int EarnDiscount { get; set; }
        public int UseDiscount { get; set; }
        public int LoseDiscount { get; set; }
        public int RemainingDiscount { get; set; }
        public int ThisPeriodDiscount { get; set; }
        public SubscriberStateTypes SubscriberState { get; set; }
    }
    public enum SubscriberStateTypes
    {
        Active = 1,
        Passive = 2
    }
}
