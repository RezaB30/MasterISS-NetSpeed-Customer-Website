using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class CustomerDocumentsViewModel
    {
        public string FileName { get; set; }
        public string FileExtention { get; set; }
        public string ServerSideName { get; set; }
        public string MIMEType { get; set; }
    }
}
