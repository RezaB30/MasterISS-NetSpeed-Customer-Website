using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class TariffsViewModel
    {
        public int TariffID { get; set; }
        public string TariffName { get; set; }
        [UIHint("Currency")]
        public string Price { get; set; }
        public string Speed { get; set; }
        [UIHint("FormattedSpeed")]
        public int? AvailabilitySpeed { get; set; }
    }
}
