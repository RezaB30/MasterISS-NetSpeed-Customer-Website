using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class CustomerLineDetailsVM
    {
        public string XDSLNo { get; set; }
        public string LineState { get; set; }   //online
        public string PhysicalState { get; set; } //açık
        public string DownloadSpeed { get; set; }
        public string UploadSpeed { get; set; }
        public string IPAddress { get; set; }
        public string XDSLType { get; set; }
    }
}
