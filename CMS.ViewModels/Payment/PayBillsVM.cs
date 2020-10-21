using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModels.Payment
{
    public class PayBillsVM
    {
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "CCNameRequired")]
        [Display(Name = "CCName" , ResourceType = typeof(CMS.Localization.Models.Models))]
        public string CCFullName { get; set; }
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "CCNumberRequired")]
        [Display(Name = "CCNumber", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string CCNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "CCMonthRequired")]
        [Display(Name = "CCMonth", ResourceType = typeof(CMS.Localization.Models.Models))]
        public int ExpirationMonthId { get; set; }
        public List<SelectListItem> ExpirationMonthDate
        {
            get
            {
                var list = new List<SelectListItem>();
                for (int i = 1; i < 13; i++)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }
                return list;
            }
        }
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "CCYearRequired")]
        [Display(Name = "CCYear", ResourceType = typeof(CMS.Localization.Models.Models))]
        public int ExpirationYearId { get; set; }
        public List<SelectListItem> ExpirationYearDate
        {
            get
            {
                var list = new List<SelectListItem>();
                for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 11; i++)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }
                return list;
            }
        }
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "CreditSecureCodeRequired")]
        [Display(Name = "CreditSecureCode", ResourceType = typeof(CMS.Localization.Models.Models))]
        public int CreditSecureCode { get; set; }
    }
}
