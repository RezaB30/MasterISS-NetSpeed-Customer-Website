using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Payment
{
    public class NewCardVM
    {
        [Required(ErrorMessageResourceType =typeof(CMS.Localization.Errors) , ErrorMessageResourceName = "CCNameRequired")]
        [Display(Name = "CCName", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string CCName { get; set; }
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "CCNumberRequired")]
        [Display(Name = "CCNumber", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string CCNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "CCMonthRequired")]
        [Display(Name = "CCMonth", ResourceType = typeof(CMS.Localization.Models.Models))]
        public int CCMonth { get; set; }
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "CCYearRequired")]
        [Display(Name = "CCYear", ResourceType = typeof(CMS.Localization.Models.Models))]
        public int CCYear { get; set; }
    }
}
