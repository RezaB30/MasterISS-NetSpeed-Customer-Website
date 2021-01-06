using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Auth
{
    public class LoginViewModel
    {
        //[Required(ErrorMessageResourceName = "LoginUsernameRequired",ErrorMessageResourceType =typeof(CMS.Localization.Errors))]
        //public string username { get; set; }
        //[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(CMS.Localization.Errors))]
        //public string password { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Common), Name = "LoginCustomerCode")]
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "Required")]
        public string CustomerCode { get; set; }

        [Display(ResourceType = typeof(CMS.Localization.Common), Name = "LoginSMSPassword")]
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "SMSPasswordWrong")]
        public string SMSPassword { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Common), Name = "Password")]
        [Required(ErrorMessageResourceType = typeof(CMS.Localization.Errors), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }
    }
}
