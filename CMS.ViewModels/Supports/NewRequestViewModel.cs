using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModels.Supports
{
    public class NewRequestViewModel
    {
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "SupportRequestType")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(CMS.Localization.Errors))]
        public int? RequestTypeId { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "SupportRequestSubType")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(CMS.Localization.Errors))]
        public int? SubRequestTypeId { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Common), Name = "Description")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(CMS.Localization.Errors))]
        public string Description { get; set; }
        public override string ToString()
        {
            return string.Join(Environment.NewLine, new string[]
            {
                $"RequestTypeId : {RequestTypeId}",
                $"SubRequestTypeId : {SubRequestTypeId}",
                $"Description : {Description}"
            });
        }
    }
}
