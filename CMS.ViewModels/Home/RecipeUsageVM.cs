using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class RecipeUsageVM
    {
        public string TariffName { get; set; }
        public string QuotaType { get; set; }
        public string CommitmentType { get; set; }
        public string TariffAmount { get; set; }
        public IEnumerable<RecipeUsageList> RecipeUsage { get; set; }
    }
    public class RecipeUsageList
    {
        [Display(Name = "TotalDownload", ResourceType = typeof(CMS.Localization.Models.Models))]
        public decimal TotalDownload { get; set; }
        [Display(Name = "TotalUpload", ResourceType = typeof(CMS.Localization.Models.Models))]
        public decimal TotalUpload { get; set; }
        public bool IsQuota { get; set; }
        public decimal TotalQuota { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public int PercentQuota { get; set; }
    }
}
