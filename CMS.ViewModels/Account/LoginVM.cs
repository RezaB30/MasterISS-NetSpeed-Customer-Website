using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Account
{
    public class LoginVM
    {
        [Required(ErrorMessageResourceName = "LoginUsernameRequired",ErrorMessageResourceType =typeof(CMS.Localization.Errors))]
        public string username { get; set; }
        [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(CMS.Localization.Errors))]
        public string password { get; set; }
    }
}
