using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class SubcriptionRegisterViewModel
    {
        [Display(ResourceType = typeof(CMS.Localization.Models.Models) , Name = "ServiceID")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        public int? ServiceID { get; set; }
        public AddressInfo SetupAddress { get; set; }
        //public int? BillingPeriod { get; set; }
        //public CustomerCommitmentInfo CommitmentInfo { get; set; }
        public ReferralDiscountInfo ReferralDiscount { get; set; }
    }
    //public class CustomerCommitmentInfo
    //{
    //    [Display(Name = "CommitmentLength", ResourceType = typeof(CMS.Localization.Models.Models))]
    //    [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
    //    public int? CommitmentLength { get; set; } //CommitmentLength
    //    //[Display(Name = "CommitmentExpirationDate", ResourceType = typeof(CMS.Localization.Models.Models))]
    //    //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
    //    //public DateTime? CommitmentExpirationDate { get; set; }
    //}
    public class ReferralDiscountInfo
    {
        [Display(Name = "ReferenceNo", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string ReferenceNo { get; set; }
    }
    public class AddressInfo
    {
        public string AddressText { get; set; }

        public string StreetName { get; set; }

        public string NeighbourhoodName { get; set; }

        public string DistrictName { get; set; }

        public string ProvinceName { get; set; }

        public long? AddressNo { get; set; }

        [Display(Name = "Floor", ResourceType = typeof(CMS.Localization.Models.Models))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        public string Floor { get; set; }
        [Display(Name = "PostalCode", ResourceType = typeof(CMS.Localization.Models.Models))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        [RegularExpression("^[0-9]+$", ErrorMessageResourceName = "NotValid", ErrorMessageResourceType = typeof(CMS.Localization.Errors))]
        public string PostalCode { get; set; } // int?
        public string ApartmentNo { get; set; }

        [Display(Name = "ApartmentID", ResourceType = typeof(CMS.Localization.Models.Models))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        public long? ApartmentID { get; set; }

        [Display(Name = "DoorID", ResourceType = typeof(CMS.Localization.Models.Models))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        public long? DoorID { get; set; }

        [Display(Name = "StreetID", ResourceType = typeof(CMS.Localization.Models.Models))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        public long? StreetID { get; set; }

        [Display(Name = "NeighbourhoodID", ResourceType = typeof(CMS.Localization.Models.Models))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        public long? NeighbourhoodID { get; set; }

        [Display(Name = "RuralCode", ResourceType = typeof(CMS.Localization.Models.Models))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        public long? RuralCode { get; set; }

        [Display(Name = "DistrictID", ResourceType = typeof(CMS.Localization.Models.Models))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        public long? DistrictID { get; set; }

        [Display(Name = "ProvinceID", ResourceType = typeof(CMS.Localization.Models.Models))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Errors))]
        public long? ProvinceID { get; set; }

        public string DoorNo { get; set; }
    }
}
