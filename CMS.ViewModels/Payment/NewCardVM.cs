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
        [Required(ErrorMessage = "Kredi Kartı Adı Gerekli")]
        public string CCName { get; set; }
        [Required(ErrorMessage = "Kredi Kartı Numarası Gerekli")]
        public string CCNumber { get; set; }
        [Required(ErrorMessage = "Kredi Kartı Ayı Gerekli")]
        public int CCMonth { get; set; }
        [Required(ErrorMessage = "Kredi Kartı Yılı Gerekli")]
        public int CCYear { get; set; }
    }
}
