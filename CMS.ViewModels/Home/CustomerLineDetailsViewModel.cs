using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class CustomerLineDetailsViewModel
    {
        [Display(Name = "XDSLNo", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string XDSLNo { get; set; }
        [Display(Name = "LineState", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string LineState { get; set; }   //online
        public string LineStateValue { get; set; }   //online
        //[Display(Name = "PhysicalState", ResourceType = typeof(CMS.Localization.Models.Models))]
        //public string PhysicalState { get; set; } //açık
        [Display(Name = "DownloadSpeed", ResourceType = typeof(CMS.Localization.Models.Models))]
        [UIHint("TextWithPlaceholder")]
        public string DownloadSpeed { get; set; }
        [Display(Name = "UploadSpeed", ResourceType = typeof(CMS.Localization.Models.Models))]
        [UIHint("TextWithPlaceholder")]
        public string UploadSpeed { get; set; }
        [Display(Name = "IPAddress", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string IPAddress { get; set; }
        [Display(Name = "XDSLType", ResourceType = typeof(CMS.Localization.Models.Models))]
        public string XDSLType { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "DownloadNoiseMargin")]
        public string DownloadMargin { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Models.Models), Name = "UploadNoiseMargin")]
        public string UploadMargin { get; set; }
    }
}
