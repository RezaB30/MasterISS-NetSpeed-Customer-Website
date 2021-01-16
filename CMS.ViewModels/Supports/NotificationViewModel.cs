using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Supports
{
    public class NotificationViewModel
    {
        public string Url { get; set; }
        public string Content { get; set; }
        public NotificationType Type { get; set; }
    }
    public enum NotificationType
    {
        Success = 1,
        Info = 2,
        Error = 3,
        Warning = 4,
    }
}
