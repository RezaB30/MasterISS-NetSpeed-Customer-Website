using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class SaveClientAttachmentViewModel
    {
        public byte[] FileContent { get; set; }    
        public string FileExtention { get; set; }
        public int? AttachmentType { get; set; }
        public long? SubscriptionId { get; set; }
    }
}
