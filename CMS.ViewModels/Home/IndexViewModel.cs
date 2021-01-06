using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class IndexViewModel
    {
        public long ID { get; set; }
        public string FullName { get; set; }
        public string SubscriberNo { get; set; }
        public string LineState { get; set; }
        public string ReferenceCode { get; set; }
        public string TariffName { get; set; }
        [UIHint("FormattedBytes")]
        public decimal DownloadUsage { get; set; }
        [UIHint("FormattedBytes")]
        public decimal UploadUsage { get; set; }
        public int UnPaidBillCount { get; set; }

    }
}
