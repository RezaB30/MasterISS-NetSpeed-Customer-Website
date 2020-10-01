using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class IndexVM
    {
        public long ID { get; set; }
        public string FullName { get; set; }
        public string SubscriberNo { get; set; }
        public string LineState { get; set; }
        public string ReferenceCode { get; set; }
        public string TariffName { get; set; }
        public string DownloadUsage { get; set; }
        public string UploadUsage { get; set; }
        public int UnPaidBillCount { get; set; }

    }
}
