using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CMS.ViewModels.Home;

namespace CustomerManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var index = new CMS.ViewModels.Home.IndexVM()
            {
                FullName = "Onur Civanoğlu",
                LineState = "Online",
                SubscriberNo = "1234567890",
                ID = 12312,
                ReferenceCode = "WAQSA1",
                TariffName = "NET EKO 8M E KADAR",
                DownloadUsage = "390 GB",
                UploadUsage = "90 GB",
                UnPaidBillCount = 2
            };
            return View(index);
        }
        public ActionResult CustomerLineDetails() //hat-durumu.html
        {
            CustomerLineDetailsVM customer = new CustomerLineDetailsVM()
            {
                DownloadSpeed = "16 MBPS",
                UploadSpeed = "8 MBPS",
                IPAddress = "127.0.0.1",
                XDSLType = "VDSL",
                XDSLNo = "8827624008",
                PhysicalState = "AÇIK",
                LineState = "ONLINE"
            };
            return View(customer);
        }
        public ActionResult RecipeUsage() // tarife-kullanim.html
        {
            var recipe = new RecipeUsageVM();
            var recipeList = new List<RecipeUsageList>();
            recipeList.Add(new RecipeUsageList()
            {
                IsQuota = true,
                Month = DateTime.Now.ToString("MMMM", Thread.CurrentThread.CurrentUICulture),
                Year = DateTime.Now.Year,
                TotalDownload = 312,
                TotalUpload = 40,
                TotalQuota = 500,
                PercentQuota = 100 - (352 * 100 / 500)
            });
            recipeList.Add(new RecipeUsageList()
            {
                IsQuota = false,
                Month = DateTime.Now.AddMonths(-1).ToString("MMMM", Thread.CurrentThread.CurrentUICulture),
                Year = DateTime.Now.Year,
                TotalDownload = 312,
                TotalUpload = 40,
                TotalQuota = 0,
                PercentQuota = 0
            });
            recipeList.Add(new RecipeUsageList()
            {
                IsQuota = true,
                Month = DateTime.Now.AddMonths(-2).ToString("MMMM", Thread.CurrentThread.CurrentUICulture),
                Year = DateTime.Now.Year,
                TotalDownload = 450,
                TotalUpload = 40,
                TotalQuota = 500,
                PercentQuota = 100 - (490 * 100 / 500)
            });
            recipeList.Add(new RecipeUsageList()
            {
                IsQuota = false,
                Month = DateTime.Now.AddMonths(-3).ToString("MMMM", Thread.CurrentThread.CurrentUICulture),
                Year = DateTime.Now.Year,
                TotalDownload = 312,
                TotalUpload = 40,
                TotalQuota = 0,
                PercentQuota = 0
            });
            recipe.CommitmentType = "TAAHHÜTSÜZ";
            recipe.QuotaType = "LİMİTSİZ";
            recipe.TariffAmount = "66 TL";
            recipe.TariffName = " NET EKO 8M E KADAR";
            recipe.RecipeUsage = recipeList;

            return View(recipe);
        }
        public ActionResult Documents() // belgelerim.html
        {
            return View();
        }
        public ActionResult MyAccount() //hesabim.html
        {
            return View();
        }
        public ActionResult MyServices() //hizmetlerim.html
        {
            return View();
        }
        public ActionResult SpecialOffer() // arkadasini-getir.html
        {
            var list = new List<SpecialOfferList>();
            for (int i = 1; i < 6; i++)
            {
                list.Add(new SpecialOfferList()
                {
                    BeginDate = DateTime.Now.AddMonths(-i),
                    EndDate = DateTime.Now.AddMonths(-i).AddYears(1),
                    EarnDiscount = 12,
                    LoseDiscount = i,
                    UseDiscount = 1,
                    RemainingDiscount = 12 - (i + 1),
                    ReferenceNo = "54DSA" + i.ToString(),
                    SubscriberState = i % 3 == 0 ? SubscriberStateTypes.Passive : SubscriberStateTypes.Active,
                    ThisPeriodDiscount = i % 3 == 0 ? 0 : 1
                });
            }
            SpecialOfferVM specialOffer = new SpecialOfferVM()
            {
                ReferenceNo = "54DSA8",
                SpecialOfferList = list,
                TotalEarnDiscount = 12 * 5,
                TotalLoseDiscount = 5 * 6 / 2,
                TotalRemainingDiscount = 6 + 7 + 8 + 9 + 10,
                TotalThisPeriod = 4,
                TotalUseDiscount = 5
            };
            return View(specialOffer);
        }
        public ActionResult SpecialOfferDetails() // arkadasini-getir-sss.html
        {
            return View(new SpecialOfferDetailsVM() { ReferenceNo = "54DSA8" });
        }
        public ActionResult SubscriptionList()
        {
            var list = new List<SelectListItem>();
            for (int i = 0; i < 3; i++)
            {
                list.Add(new SelectListItem()
                {
                    Text = "12226626972",
                    Value = "12226626972"
                });
            }
            return PartialView("~/Views/Shared/EditorTemplates/Home/SubscriptionList.cshtml", new SubscriptionListVM() { SubscriptionList = list });
        }
        public ActionResult CurrentSubscription(SubscriptionListVM subscription)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}