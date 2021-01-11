using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CMS.ViewModels.Home;
using RezaB.Web.Authentication;

namespace CustomerManagementSystem.Controllers
{
    public class HomeController : BaseController
    {
        GenericCustomerServiceReference.GenericCustomerServiceClient client = new GenericCustomerServiceReference.GenericCustomerServiceClient();
        public ActionResult Index()
        {
            var customerInfo = new ServiceUtilities().GetCustomerInfo(User.GiveUserId());
            if (customerInfo.ResponseMessage.ErrorCode != 0)
            {
                return View(new IndexViewModel());
            }
            var tariffAndtraffic = new ServiceUtilities().GetTariffAndTrafficInfo(User.GiveUserId());
            var tariffInfo = tariffAndtraffic.GetCustomerTariffAndTrafficInfoResponse;
            var bills = new ServiceUtilities().GetCustomerBillList(User.GiveUserId());
            var index = new IndexViewModel()
            {
                FullName = customerInfo.GetCustomerInfoResponse.ValidDisplayName,
                LineState = "Online",
                SubscriberNo = customerInfo.GetCustomerInfoResponse.CurrentSubscriberNo,
                ID = User.GiveUserId().Value,
                ReferenceCode = customerInfo.GetCustomerInfoResponse == null ? "-" : customerInfo.GetCustomerInfoResponse.ReferenceNo,
                TariffName = tariffInfo == null ? "-" : tariffInfo.ServiceName,
                DownloadUsage = tariffInfo == null ? 0 : tariffInfo.Download,
                UploadUsage = tariffInfo == null ? 0 : tariffInfo.Upload,
                UnPaidBillCount = bills.ResponseMessage.ErrorCode != 0 ? 0 : bills.GetCustomerBillsResponse.CustomerBills.Where(bill => bill.Status == 1).Count()
            };
            return View(index);
        }
        public ActionResult CustomerLineDetails() //hat-durumu.html
        {
            if (Request.IsAjaxRequest())
            {
                var lineDetails = new ServiceUtilities().ConnectionStatus(User.GiveUserId());
                if (lineDetails.ResponseMessage.ErrorCode != 0)
                {
                    return PartialView("_ConnectionStatusPartial", new CustomerLineDetailsViewModel());
                }
                CustomerLineDetailsViewModel customer = new CustomerLineDetailsViewModel()
                {
                    DownloadSpeed = lineDetails.GetCustomerConnectionStatusResponse.CurrentDownload,
                    UploadSpeed = lineDetails.GetCustomerConnectionStatusResponse.CurrentUpload,
                    IPAddress = Utilities.InternalUtilities.GetUserIP(),
                    XDSLType = lineDetails.GetCustomerConnectionStatusResponse.XDSLTypeText,
                    XDSLNo = lineDetails.GetCustomerConnectionStatusResponse.XDSLNo,
                    LineState = lineDetails.GetCustomerConnectionStatusResponse.ConnectionStatusText,
                    DownloadMargin = lineDetails.GetCustomerConnectionStatusResponse.DownloadMargin,
                    UploadMargin = lineDetails.GetCustomerConnectionStatusResponse.UploadMargin
                };
                return PartialView("_ConnectionStatusPartial", customer);
            }
            else
            {
                return View(new CustomerLineDetailsViewModel());
            }
        }
        public ActionResult RecipeUsage() // tarife-kullanim.html
        {
            var tariffAndtraffic = new ServiceUtilities().GetTariffAndTrafficInfo(User.GiveUserId());
            var tariffInfo = tariffAndtraffic.GetCustomerTariffAndTrafficInfoResponse;
            if (tariffInfo == null)
            {
                return View(new RecipeUsageViewModel());
            }
            var recipe = new RecipeUsageViewModel();

            var recipeList = new List<RecipeUsageList>();
            foreach (var item in tariffInfo.MonthlyUsage)
            {
                recipeList.Add(new RecipeUsageList()
                {
                    IsQuota = tariffInfo.BaseQuota != null,
                    Month = new DateTime(item.Year.GetValueOrDefault(), item.Month.GetValueOrDefault(), 1),
                    Year = new DateTime(item.Year.GetValueOrDefault(), item.Month.GetValueOrDefault(), 1),
                    TotalDownload = item.TotalDownload,
                    TotalUpload = item.TotalUpload,
                    TotalQuota = tariffInfo.BaseQuota,
                    PercentQuota = tariffInfo.BaseQuota != null ? 100 - ((item.TotalUpload + item.TotalDownload) * 100 / tariffInfo.BaseQuota.Value) : 0
                });
            }
            //recipe.CommitmentType = "TAAHHÜTSÜZ";
            //recipe.QuotaType = "LİMİTSİZ";
            //recipe.TariffAmount = "66 TL";
            recipe.TariffName = tariffInfo.ServiceName;
            recipe.RecipeUsage = recipeList.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).ToArray();

            return View(recipe);
        }
        public ActionResult Documents() // belgelerim.html
        {
            return View();
        }
        public ActionResult MyAccount() //hesabim.html
        {
            var getCustomerInfo = new ServiceUtilities().GetCustomerInfo(User.GiveUserId());
            if (getCustomerInfo.ResponseMessage.ErrorCode != 0)
            {
                return View(new MyAccountViewModel());
            }
            var customer = getCustomerInfo.GetCustomerInfoResponse;
            var getBillInfo = new ServiceUtilities().GetCustomerBillList(User.GiveUserId());
            int? billCount = null;
            if (getBillInfo.ResponseMessage.ErrorCode == 0)
            {
                billCount = getBillInfo.GetCustomerBillsResponse.CustomerBills.Where(bill => bill.Status == (short)CMS.Localization.Enums.BillState.Unpaid).Count();
            }
            var account = new MyAccountViewModel()
            {
                AutomaticPaymentInstruction = customer.HashAutoPayment,
                ChooseMail = true,
                ChooseSMS = false,
                ContactPhoneNo = customer.PhoneNo,
                CustomerServicePassword = customer.OnlinePassword,
                FullName = customer.ValidDisplayName,
                LineAddress = customer.InstallationAddress,
                Mail = customer.EMail,
                ModemPassword = customer.Username,
                ModemUsername = customer.Password,
                PassedInvoice = billCount,
                SpecialOfferReferenceCode = customer.ReferenceNo,
                StaticIP = customer.StaticIP,
                SubscriberNo = customer.CurrentSubscriberNo,
                SubscriptionState = customer.CustomerStateText,
                XDSLNo = customer.TTSubscriberNo
            };
            return View(account);
        }
        public ActionResult MyServices() //hizmetlerim.html
        {
            return View();
        }
        public ActionResult SpecialOffer() // arkadasini-getir.html
        {
            var customerSpecialOffer = new ServiceUtilities().CustomerSpecialOffers(User.GiveUserId());
            var customerInfo = new ServiceUtilities().GetCustomerInfo(User.GiveUserId());

            if (customerSpecialOffer.ResponseMessage.ErrorCode != 0)
            {
                return View(new SpecialOfferViewModel());
            }
            var specialOfflerList = customerSpecialOffer.GetCustomerSpecialOffersResponse.Select(s => new SpecialOfferList()
            {
                StartDate = s.StartDate,
                IsCancelled = s.IsCancelled,
                MissedCount = s.MissedCount,
                ReferenceNo = s.ReferenceNo,
                ReferralSubscriberState = s.ReferralSubscriberState,
                TotalCount = s.TotalCount,
                UsedCount = s.UsedCount
            });
            SpecialOfferViewModel specialOffer = new SpecialOfferViewModel()
            {
                ReferenceNo = customerInfo.ResponseMessage.ErrorCode != 0 ? string.Empty : customerInfo.GetCustomerInfoResponse.ReferenceNo,
                SpecialOfferList = specialOfflerList,
                TotalEarnDiscount = specialOfflerList.Sum(s => s.TotalCount),
                TotalLoseDiscount = specialOfflerList.Sum(s => s.MissedCount),
                TotalRemainingDiscount = specialOfflerList.Sum(s => s.RemainingCount),
                TotalThisPeriod = specialOfflerList.Where(s => s.IsApplicableThisPeriod).Count(),
                TotalUseDiscount = specialOfflerList.Sum(s => s.UsedCount)
            };
            return View(specialOffer);
        }
        public ActionResult SpecialOfferDetails() // arkadasini-getir-sss.html
        {
            var customerInfo = new ServiceUtilities().GetCustomerInfo(User.GiveUserId());

            return View(new SpecialOfferDetailsViewModel()
            {
                ReferenceNo = customerInfo.ResponseMessage.ErrorCode != 0 ? string.Empty
                : customerInfo.GetCustomerInfoResponse.ReferenceNo
            });
        }
        public ActionResult SubscriptionList()
        {
            var list = new List<SelectListItem>();
            var subscriptionBag = Utilities.InternalUtilities.GetSubscriptionBag(User);
            foreach (var item in subscriptionBag)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.SubscriberNo,
                    Value = item.ID
                });
            }
            return PartialView("~/Views/Shared/EditorPartials/Home/SubscriptionList.cshtml", new SubscriptionListVM() { SubscriptionList = list });
        }
        public ActionResult CurrentSubscription(SubscriptionListVM subscription) // abone numarasını değiştirmek isterse
        {
            var targetId = Convert.ToInt64(subscription.CurrentSubscriberNo);
            var response = new ServiceUtilities().ChangeSubClient(User.GiveUserId(), targetId);
            if (response.ResponseMessage.ErrorCode != 0)
            {
                //log
                return RedirectToAction("Index", "Home");
            }
            AuthController.SignoutUser(Request.GetOwinContext());
            AuthController.SignInUser(response.ChangeSubClientResponse.ValidDisplayName,
                response.ChangeSubClientResponse.ID.ToString(),
                response.ChangeSubClientResponse.SubscriberNo,
                response.ChangeSubClientResponse.RelatedCustomers,
                Request.GetOwinContext());
            return RedirectToAction("Index", "Home");
        }
        public ActionResult QuickSearch(string query)
        {
            var SearchList = new List<QuickSearch>();
            SearchList.Add(new CMS.ViewModels.Home.QuickSearch()
            {
                Content = "oto",
                Header = "Otomatik Ödeme Talimatı",
                Url = "/Payment/PaymentInstruction"
            });
            SearchList.Add(new CMS.ViewModels.Home.QuickSearch()
            {
                Content = "öde",
                Header = "Otomatik Ödeme Talimatı",
                Url = "/Payment/PaymentInstruction"
            });
            SearchList.Add(new CMS.ViewModels.Home.QuickSearch()
            {
                Content = "öde",
                Header = "Fatura & Ödemeler",
                Url = "/Payment/Bills"
            });
            SearchList.Add(new CMS.ViewModels.Home.QuickSearch()
            {
                Content = "fat",
                Header = "Otomatik Ödeme Talimatı",
                Url = "/Payment/PaymentInstruction"
            });
            SearchList.Add(new CMS.ViewModels.Home.QuickSearch()
            {
                Content = "fat",
                Header = "Fatura & Ödemeler",
                Url = "/Payment/Bills"

            });
            var contentList = new List<string>();
            foreach (var item in SearchList.Distinct())
            {
                if (query.Contains(item.Content))
                {
                    contentList.Add("<div class='d-flex align-items-center flex-grow-1 mb-2'>" +
                        "<div class='symbol symbol-30 bg-transparent flex-shrink-0'>" +
                        "<img src='/assets/media/svg/icons/Navigation/Arrow-right.svg' alt='' >" +
                        "</div>" +
                        "<div class='d-flex flex-column ml-3 mt-2 mb-2'>" +
                        "<a href='" + item.Url + "' class='font-weight-bold text-dark text-hover-primary'>" + item.Header + "</a>" +
                        "</div>" + "</div>");
                }
            }
            var content = "<div class='quick-search-result'>" +
                "<div class='text-muted d-none'>No record found</div>" +
                "<div class='font-size-sm text-primary font-weight-bolder text-uppercase mb-2'>Arama Sonuçları</div>" +
                "<div class='mb-10'>" + string.Join("", contentList.ToArray()) +
                "</div>" +
                "</div>";
            return Content(content);
        }
    }
}