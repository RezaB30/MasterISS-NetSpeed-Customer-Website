using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Home
{
    public class RecipeUsageViewModel
    {
        public RecipeUsageViewModel()
        {
            RecipeUsage = Enumerable.Empty<RecipeUsageList>();
        }
        public string TariffName { get; set; }
        //public string QuotaType { get; set; }
        //public string CommitmentType { get; set; }
        //public string TariffAmount { get; set; }
        public IEnumerable<RecipeUsageList> RecipeUsage { get; set; }
    }
    public class RecipeUsageList
    {
        [Display(Name = "TotalDownload", ResourceType = typeof(CMS.Localization.Models.Models))]
        [UIHint("FormattedBytes")]
        public decimal TotalDownload { get; set; }
        [Display(Name = "TotalUpload", ResourceType = typeof(CMS.Localization.Models.Models))]
        [UIHint("FormattedBytes")]
        public decimal TotalUpload { get; set; }
        public bool IsQuota { get; set; }
        public decimal? TotalQuota { get; set; }
        [UIHint("Month")]
        public DateTime? Month { get; set; }
        [UIHint("Year")]
        public DateTime? Year { get; set; }
        public long PercentQuota { get; set; }
    }
}
