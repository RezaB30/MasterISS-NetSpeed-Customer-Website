using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModels.Home
{
    public class SubscriptionListVM
    {
        public string CurrentSubscriberNo { get; set; }
        public IEnumerable<SelectListItem> SubscriptionList { get; set; }
    }
}
