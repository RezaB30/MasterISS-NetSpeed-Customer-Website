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
        [Required(ErrorMessage = "TC Kimlik No veya GSM No Gerekli")]
        public string username { get; set; }
        [Required(ErrorMessage = "Şifre Gerekli")]
        public string password { get; set; }
    }
}
