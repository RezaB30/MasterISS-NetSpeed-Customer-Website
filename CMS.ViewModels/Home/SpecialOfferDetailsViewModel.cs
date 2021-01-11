using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class SpecialOfferDetailsViewModel
    {
        [UIHint("TextWithPlaceholder")]
        public string ReferenceNo { get; set; }
    }
}
