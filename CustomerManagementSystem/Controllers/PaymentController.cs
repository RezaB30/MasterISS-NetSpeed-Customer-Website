using CMS.ViewModels.Payment;
//using CustomerManagementSystem.GenericCustomerServiceReference;
using MasterISS.CustomerService.GenericCustomerServiceReference;
using CustomerManagementSystem.VPOSToken;
using NLog;
using PagedList;
using RezaB.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class PaymentController : BaseController
    {
        Logger paymentLogger = LogManager.GetLogger("payments");
        Logger unpaidLogger = LogManager.GetLogger("unpaid");
        Logger generalLogger = LogManager.GetLogger("general");

        public ActionResult BillsAndPayments(int? page) // fatura-odeme.html
        {
            var response = new ServiceUtilities().GetCustomerBillList(User.GiveUserId());
            if (response.ResponseMessage.ErrorCode != 0)
            {
                return View(Enumerable.Empty<PaymentsAndBillsViewModel>());
            }

            var results = response.GetCustomerBillsResponse.CustomerBills.OrderByDescending(bill => bill.BillDate).Select(bill => new PaymentsAndBillsViewModel()
            {
                ID = bill.ID,
                ServiceName = bill.ServiceName,
                BillDate = bill.BillDate,
                LastPaymentDate = bill.LastPaymentDate,
                Total = bill.Total.ToString("###,##0.00"),
                Status = bill.Status,
                CanBePaid = bill.CanBePaid,
                HasEArchiveBill = bill.HasEArchiveBill
            }).AsQueryable();
            SetupPages(page, ref results, 10);
            ViewBag.HasUnpaidBills = response.GetCustomerBillsResponse.HasUnpaidBills;
            ViewBag.IsPrePaid = response.GetCustomerBillsResponse.IsPrePaid;
            //quota
            if (response.GetCustomerBillsResponse.CanHaveQuotaSale)
            {
                ViewBag.CanBuyQuota = true;
                var QuotaListResponse = new ServiceUtilities().QuotaPackageList();
                ViewBag.QuotaPackages = QuotaListResponse.ResponseMessage.ErrorCode != 0 ? Enumerable.Empty<QuotaPackageViewModel>() : QuotaListResponse.QuotaPackageListResponse.Select(q => new QuotaPackageViewModel()
                {
                    ID = q.ID,
                    _amount = q.Amount,
                    _price = q.Price,
                    Name = q.Name
                }).ToArray();
            }
            // errors
            if (!string.IsNullOrEmpty(Session["POSErrorMessage"] as string))
            {
                ViewBag.POSErrorMessage = Session["POSErrorMessage"];
                Session.Remove("POSErrorMessage");
            }
            if (!string.IsNullOrEmpty(Session["POSSuccessMessage"] as string))
            {
                ViewBag.POSSuccessMessage = Session["POSSuccessMessage"];
                Session.Remove("POSSuccessMessage");
            }
            if (TempData.ContainsKey("ServiceError"))
                ViewBag.ServiceError = TempData["ServiceError"];
            // view credits
            var credits = response.GetCustomerBillsResponse.SubscriptionCredits;
            if (credits > 0m)
                ViewBag.ClientCredits = credits;
            return View(results.ToList());
        }
        public ActionResult BuyQuota(int id)
        {
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var response = client.QuotaPackageList(new CustomerServiceQuotaPackagesRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username,
            });
            if (response.ResponseMessage.ErrorCode != 0)
            {
                return RedirectToAction("BillsAndPayments");
            }

            var quotaBaseRequest = new GenericServiceSettings();
            var quotaResponse = client.CanHaveQuotaSale(new CustomerServiceBaseRequest()
            {
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = User.GiveUserId()
                },
                Culture = quotaBaseRequest.Culture,
                Hash = quotaBaseRequest.Hash,
                Username = quotaBaseRequest.Username,
                Rand = quotaBaseRequest.Rand
            });
            if (quotaResponse.ResponseMessage.ErrorCode != 0)
            {
                return RedirectToAction("BillsAndPayments");
            }
            var tokenKey = VPOSTokenManager.RegisterPaymentToken(new QuotaSaleToken()
            {
                SubscriberId = User.GiveUserId().Value,
                PackageID = id
            });
            var vposBaseRequest = new GenericServiceSettings();
            var quotaPrice = response.QuotaPackageListResponse.Where(q => q.ID == id).FirstOrDefault();
            var vposResponse = client.GetVPOSForm(new CustomerServiceVPOSFormRequest()
            {
                Culture = vposBaseRequest.Culture,
                Hash = vposBaseRequest.Hash,
                Rand = vposBaseRequest.Rand,
                Username = vposBaseRequest.Username,
                VPOSFormParameters = new VPOSFormRequest()
                {
                    FailUrl = Url.Action("VPOSFail", null, new { id = tokenKey }, Request.Url.Scheme),
                    OkUrl = Url.Action("VPOSSuccess", null, new { id = tokenKey }, Request.Url.Scheme),
                    PayableAmount = quotaPrice.Price,
                    SubscriptionId = User.GiveUserId(),
                }
            });
            if (vposResponse.ResponseMessage.ErrorCode != 0)
            {
                return RedirectToAction("BillsAndPayments");
            }
            ViewBag.POSForm = vposResponse.VPOSFormResponse.HtmlForm;
            return View(viewName: "3DHostPayment");
        }
        public ActionResult AutomaticPayment()
        {
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var settings = GenericAppSettings();
            if (settings.ResponseMessage.ErrorCode != 0 || !settings.GenericAppSettings.MobilExpressIsActive)
            {
                return RedirectToAction("BillsAndPayments");
            }
            var cardBaseRequest = new GenericServiceSettings();
            var cardList = client.RegisteredMobilexpressCardList(new CustomerServiceRegisteredCardsRequest()
            {
                Culture = cardBaseRequest.Culture,
                Hash = cardBaseRequest.Hash,
                Rand = cardBaseRequest.Rand,
                Username = cardBaseRequest.Username,
                RegisteredCardsParameters = new RegisteredCardsRequest()
                {
                    HttpContextParameters = new HttpContextParameters()
                    {
                        UserAgent = Request.UserAgent,
                        UserHostAddress = Request.UserHostAddress.ToString()
                    },
                    SubscriptionId = User.GiveUserId()
                }
            });
            if (cardList.ResponseMessage.ErrorCode != 0)
            {
                generalLogger.Warn($"Error calling 'GetCards' from MobilExpress client. Code : {cardList.ResponseMessage.ErrorCode} - Message : {cardList.ResponseMessage.ErrorMessage}");
                ViewBag.ServiceError = CMS.Localization.Errors.GeneralError;
                return View();
            }
            var cards = cardList.RegisteredCardList == null ? Enumerable.Empty<CustomerAutomaticPaymentViewModel.CardViewModel>() : cardList.RegisteredCardList.Select(cl => new CustomerAutomaticPaymentViewModel.CardViewModel()
            {
                HasAutoPayments = cl.HasAutoPayments,
                Token = cl.Token,
                MaskedCardNo = cl.MaskedCardNo
            }).ToArray();
            var autoPaymentsBaseRequest = new GenericServiceSettings();
            var autoPaymentList = client.AutoPaymentList(new CustomerServiceAutoPaymentListRequest()
            {
                Culture = autoPaymentsBaseRequest.Culture,
                Hash = autoPaymentsBaseRequest.Hash,
                Rand = autoPaymentsBaseRequest.Rand,
                Username = autoPaymentsBaseRequest.Username,
                AutoPaymentListParameters = new AutoPaymentListRequest()
                {
                    SubscriptionId = User.GiveUserId(),
                    CardList = cardList.RegisteredCardList
                }
            });
            if (autoPaymentList.ResponseMessage.ErrorCode != 0)
            {
                generalLogger.Warn($"Error calling 'remove invalid cards' from web service. Code : {autoPaymentList.ResponseMessage.ErrorCode} - Message : {autoPaymentList.ResponseMessage.ErrorMessage}");
            }
            var autoPayments = autoPaymentList.AutoPaymentListResult.Select(s => new CustomerAutomaticPaymentViewModel.AutomaticPaymentViewModel()
            {
                SubscriberID = s.SubscriberID,
                SubscriberNo = s.SubscriberNo,
                Card = s.Cards == null ? null : new CustomerAutomaticPaymentViewModel.CardViewModel()
                {
                    Token = s.Cards.Token,
                    MaskedCardNo = s.Cards.MaskedCardNo
                }
            }).ToArray();
            foreach (var card in cards)
            {
                card.HasAutoPayments = autoPayments.Where(ap => ap.Card != null).Any(ap => ap.Card.Token == card.Token);
            }
            return View(new CustomerAutomaticPaymentViewModel()
            {
                Cards = cards,
                AutomaticPayments = autoPayments
            });
        }
        public ActionResult AddNewCard() // kart-ekle.html
        {
            var settings = GenericAppSettings();
            if (settings.ResponseMessage.ErrorCode != 0)
            {
                return RedirectToAction("BillsAndPayments");
            }
            if (!settings.GenericAppSettings.MobilExpressIsActive)
            {
                return RedirectToAction("BillsAndPayments");
            }
            return View();
        }
        public ActionResult CreditCardRegister()
        {
            var settings = GenericAppSettings();
            if (settings.ResponseMessage.ErrorCode != 0)
            {
                return RedirectToAction("BillsAndPayments");
            }
            if (!settings.GenericAppSettings.MobilExpressIsActive)
            {
                return RedirectToAction("BillsAndPayments");
            }
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreditCardRegister(AutoPaymentCardViewModel card)
        {
            if (ModelState.IsValid)
            {
                GenericCustomerServiceClient client = new GenericCustomerServiceClient();
                var baseRequest = new GenericServiceSettings();
                var addCardSms = client.AddCardSMSCheck(new CustomerServiceBaseRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    SubscriptionParameters = new BaseSubscriptionRequest()
                    {
                        SubscriptionId = User.GiveUserId()
                    }
                });
                if (addCardSms.ResponseMessage.ErrorCode != 0)
                {
                    //error message
                    return RedirectToAction("AutomaticPayment");
                }
                TempData["smsCode"] = addCardSms.SMSCode;
                return View(viewName: "AddCardSMSCheck", model: card);
            }
            return View(card);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddCardSMSCheck(AutoPaymentCardViewModel card, string smsCode)
        {
            if (!TempData.ContainsKey("smsCode"))
            {
                return View(viewName: "CreditCardRegister", model: card);
            }
            if (smsCode != TempData["smsCode"].ToString())
            {
                var retries = TempData.ContainsKey("smsRetries") ? TempData["smsRetries"] as int? ?? 0 : 0;
                retries++;
                if (retries > 3)
                    return View(viewName: "CreditCardRegister", model: card);
                TempData.Keep("smsCode");
                TempData["smsRetries"] = retries;
                ViewBag.WrongPassword = CMS.Localization.Errors.SMSPasswordWrong;
                return View(card);
            }
            TempData.Remove("smsRetries");

            if (ModelState.IsValid)
            {
                GenericCustomerServiceClient client = new GenericCustomerServiceClient();
                var baseRequest = new GenericServiceSettings();
                var addCard = client.AddCard(new CustomerServiceAddCardRequest()
                {
                    AddCardParameters = new AddCardRequest()
                    {
                        HttpContextParameters = new HttpContextParameters()
                        {
                            UserAgent = Request.UserAgent,
                            UserHostAddress = Request.UserHostAddress.ToString()
                        },
                        CardholderName = card.CardholderName,
                        CardNo = card.CardNo,
                        ExpirationMonth = card.ExpirationMonth,
                        ExpirationYear = card.ExpirationYear,
                        SMSCode = "",
                        SubscriptionId = User.GiveUserId(),
                    },
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username
                });
                if (addCard.ResponseMessage.ErrorCode == 0)
                {
                    return RedirectToAction("AutomaticPayment");
                }
                generalLogger.Error($"Subscription ID : {User.GiveUserId()} . | {addCard.ResponseMessage.ErrorMessage}.Error calling 'SaveCard' from MobilExpress client");
                TempData["generic-error"] = CMS.Localization.Errors.GeneralError;
                return View(viewName: "CreditCardRegister", model: card);
            }

            return View(viewName: "CreditCardRegister", model: card);
        }
        [HttpPost]
        public ActionResult PrevCreditCardRegister(AutoPaymentCardViewModel card)
        {
            return View(viewName: "CreditCardRegister", model: card);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RemoveCard(string id)
        {
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var removeCardSms = client.RemoveCardSMSCheck(new CustomerServiceRemoveCardSMSCheckRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username,
                RemoveCardSMSCheckParameters = new RemoveCardSMSCheckRequest()
                {
                    CardToken = id,
                    SubscriptionId = User.GiveUserId()
                },
            });
            if (removeCardSms.ResponseMessage.ErrorCode != 0)
            {
                generalLogger.Error($"Subscription ID : {User.GiveUserId()}. | Message : {removeCardSms.ResponseMessage.ErrorMessage}.Error calling 'SaveCard' from MobilExpress client");
                return RedirectToAction("BillsAndPayments");
            }
            TempData["smsCode"] = removeCardSms.SMSCode;
            return View(viewName: "RemoveCardSMSCheck", model: id);

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RemoveCardSMSCheck(string id, string smsCode)
        {
            if (!TempData.ContainsKey("smsCode"))
            {
                return View("AutomaticPayment");
            }
            if (smsCode != TempData["smsCode"].ToString())
            {
                var retries = TempData.ContainsKey("smsRetries") ? TempData["smsRetries"] as int? ?? 0 : 0;
                retries++;
                if (retries > 3)
                    return View("AutomaticPayment");
                TempData.Keep("smsCode");
                TempData["smsRetries"] = retries;
                ViewBag.WrongPassword = CMS.Localization.Errors.SMSPasswordWrong;
                return View(model: id);
            }
            TempData.Remove("smsRetries");
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var removeCard = client.RemoveCard(new CustomerServiceRemoveCardRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                RemoveCardParameters = new RemoveCardRequest()
                {
                    CardToken = id,
                    SMSCode = "",
                    SubscriptionId = User.GiveUserId(),
                    HttpContextParameters = new HttpContextParameters()
                    {
                        UserHostAddress = Request.UserHostAddress,
                        UserAgent = Request.UserAgent
                    }
                },
                Username = baseRequest.Username
            });
            if (removeCard.ResponseMessage.ErrorCode != 0)
            {
                generalLogger.Warn($"{removeCard.ResponseMessage.ErrorMessage} - Error calling 'DeleteCard' from MobilExpress client");
                TempData["ServiceError"] = removeCard.ResponseMessage.ErrorMessage;// RadiusRCustomerWebSite.Localization.Common.GeneralError;
                return RedirectToAction("AutomaticPayment");
            }
            return RedirectToAction("AutomaticPayment");
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ActivateAutomaticPayment(/*long id, */ string token)
        {
            var settings = GenericAppSettings();
            if (settings.ResponseMessage.ErrorCode != 0 || !settings.GenericAppSettings.MobilExpressIsActive)
            {
                return RedirectToAction("BillsAndPayments");
            }
            var paymentTypes = new ServiceUtilities().PaymentTypeList();

            ViewBag.PaymentTypes = paymentTypes.PaymentTypes == null ? new SelectList(Enumerable.Empty<Dictionary<int, string>>(), "Key", "Value") : new SelectList(paymentTypes.PaymentTypes, "Key", "Value");
            return View(new ActivateAutomaticPaymentViewModel()
            {
                CardToken = token,
                SubscriptionID = User.GiveUserId().Value //id
            });
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ActivateAutomaticPaymentConfirm(ActivateAutomaticPaymentViewModel automaticPayment)
        {
            var settings = GenericAppSettings();
            if (settings.ResponseMessage.ErrorCode != 0 || !settings.GenericAppSettings.MobilExpressIsActive)
            {
                return RedirectToAction("BillsAndPayments");
            }

            if (ModelState.IsValid)
            {
                var dbSubscription = new ServiceUtilities().GetSubscriptionInfo(automaticPayment.SubscriptionID);
                var currentCustomer = new ServiceUtilities().GetSubscriptionInfo(User.GiveUserId());
                if (dbSubscription.ResponseMessage.ErrorCode != 0 || currentCustomer.ResponseMessage.ErrorCode != 0)
                {
                    return RedirectToAction("AutomaticPayment");
                }
                if (dbSubscription.SubscriptionBasicInformationResponse.CustomerID != currentCustomer.SubscriptionBasicInformationResponse.CustomerID || dbSubscription.SubscriptionBasicInformationResponse.IsCancelled)
                {
                    return RedirectToAction("AutomaticPayment");
                }
                GenericCustomerServiceClient client = new GenericCustomerServiceClient();
                var activateBaseRequest = new GenericServiceSettings();
                var activateAutomaticPayment = client.ActivateAutomaticPayment(new CustomerServiceActivateAutomaticPaymentRequest()
                {
                    Culture = activateBaseRequest.Culture,
                    Hash = activateBaseRequest.Hash,
                    Rand = activateBaseRequest.Rand,
                    Username = activateBaseRequest.Username,
                    ActivateAutomaticPaymentParameters = new ActivateAutomaticPaymentRequest()
                    {
                        CardToken = automaticPayment.CardToken,
                        HttpContextParameters = new HttpContextParameters()
                        {
                            UserHostAddress = Request.UserHostAddress,
                            UserAgent = Request.UserAgent
                        },
                        PaymentType = automaticPayment.PaymentType,
                        SubscriptionId = automaticPayment.SubscriptionID
                    }
                });
                if (activateAutomaticPayment.ResponseMessage.ErrorCode != 0)
                {
                    generalLogger.Warn($"Error calling 'GetCards' from MobilExpress client. Code : {activateAutomaticPayment.ResponseMessage.ErrorCode} - Message : {activateAutomaticPayment.ResponseMessage.ErrorMessage} ");
                    return ReturnMessageUrl(Url.Action("AutomaticPayment", "Payment"), CMS.Localization.Errors.GeneralError, false);
                }
            }

            return RedirectToAction("AutomaticPayment");
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeactivateAutomaticPayment(long id)
        {
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var settingRequest = new GenericServiceSettings();
            var settings = client.GenericAppSettings(new CustomerServiceGenericAppSettingsRequest()
            {
                Culture = settingRequest.Culture,
                Hash = settingRequest.Hash,
                Rand = settingRequest.Rand,
                Username = settingRequest.Username
            });
            if (settings.ResponseMessage.ErrorCode != 0 || !settings.GenericAppSettings.MobilExpressIsActive)
            {
                return RedirectToAction("BillsAndPayments");
            }
            var dbSubscription = new ServiceUtilities().GetSubscriptionInfo(id);
            var currentCustomer = new ServiceUtilities().GetSubscriptionInfo(User.GiveUserId());

            if (dbSubscription.ResponseMessage.ErrorCode != 0 || currentCustomer.ResponseMessage.ErrorCode != 0)
            {
                return RedirectToAction("AutomaticPayment");
            }
            if (dbSubscription.SubscriptionBasicInformationResponse.CustomerID != currentCustomer.SubscriptionBasicInformationResponse.CustomerID)
            {
                return RedirectToAction("AutomaticPayment");
            }
            var deactivateBaseRequest = new GenericServiceSettings();
            var deactiveAutomaticPayment = client.DeativateAutomaticPayment(new CustomerServiceBaseRequest()
            {
                Culture = deactivateBaseRequest.Culture,
                Hash = deactivateBaseRequest.Hash,
                Rand = deactivateBaseRequest.Rand,
                Username = deactivateBaseRequest.Username,
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = dbSubscription.SubscriptionBasicInformationResponse.ID
                }
            });
            generalLogger.Info($"Deactive Automatic Payment Result -> Code : {deactiveAutomaticPayment.ResponseMessage.ErrorCode} - Message : {deactiveAutomaticPayment.ResponseMessage.ErrorMessage}");
            return RedirectToAction("AutomaticPayment");
        }
        [HttpGet]
        public ActionResult PaymentSelection(long? id)
        {
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var settings = GenericAppSettings();
            if (settings.ResponseMessage.ErrorCode != 0 || !settings.GenericAppSettings.MobilExpressIsActive)
            {
                return RedirectToAction("Payment", new { id = id });
            }
            var payableAmount = GetPayableAmount(User.GiveUserId(), id);
            if (payableAmount == 0m)
            {
                var dbBills = new ServiceUtilities().GetCustomerBillList(User.GiveUserId());
                if (dbBills.ResponseMessage.ErrorCode != 0)
                {
                    generalLogger.Error($"Error calling 'get bills' from web service. Code : {dbBills.ResponseMessage.ErrorCode} - Message : {dbBills.ResponseMessage.ErrorMessage}");
                    return ReturnMessageUrl(Url.Action("BillsAndPayments"), CMS.Localization.Errors.InternalErrorDescription, false);
                }
                var billIDs = id.HasValue ? new[] { id.Value } : dbBills.GetCustomerBillsResponse.CustomerBills.Where(b => b.PaymentTypeID == (short)CMS.Localization.Enums.PaymentType.None).Select(b => b.ID).ToArray(); // enum paymentType
                if (billIDs.Any())
                {
                    var payBills = PayBills(billIDs, (short)CMS.Localization.Enums.SubscriptionPaidType.Billing, User.GiveUserId(), (int)CMS.Localization.Enums.PaymentType.Cash, (int)CMS.Localization.Enums.AccountantType.Admin);
                    generalLogger.Debug($"Payment selection 'Pay Bills' web service result. Code : {payBills.ResponseMessage.ErrorCode}");
                    if (payBills.ResponseMessage.ErrorCode != 0)
                    {
                        return ReturnMessageUrl(Url.Action("BillsAndPayments"), CMS.Localization.Errors.InternalErrorDescription, false);
                    }
                }

                // log - need web service

                var logBaseRequest = new GenericServiceSettings();
                var systemLog = client.PaymentSystemLog(new CustomerServicePaymentSystemLogRequest()
                {
                    Culture = logBaseRequest.Culture,
                    Hash = logBaseRequest.Hash,
                    Rand = logBaseRequest.Rand,
                    Username = logBaseRequest.Username,
                    PaymentSystemLogParameters = new PaymentSystemLogRequest()
                    {
                        BillIds = billIDs,
                        PaymentType = (int)CMS.Localization.Enums.PaymentType.Cash,
                        SubscriberNo = User.GiveClientSubscriberNo(),
                        SubscriptionId = User.GiveUserId(),
                        UserId = null
                    }
                });
                if (systemLog.ResponseMessage.ErrorCode != 0)
                {
                    generalLogger.Error($"Error Calling 'system log payment' from webservice. Code : {systemLog.ResponseMessage.ErrorCode} ");
                }
                return RedirectToAction("BillsAndPayments");
            }
            // get m.express cards
            var cardBaseRequest = new GenericServiceSettings();
            var cardList = client.RegisteredMobilexpressCardList(new CustomerServiceRegisteredCardsRequest()
            {
                Culture = cardBaseRequest.Culture,
                Hash = cardBaseRequest.Hash,
                Rand = cardBaseRequest.Rand,
                Username = cardBaseRequest.Username,
                RegisteredCardsParameters = new RegisteredCardsRequest()
                {
                    HttpContextParameters = new HttpContextParameters()
                    {
                        UserAgent = Request.UserAgent,
                        UserHostAddress = Request.UserHostAddress
                    },
                    SubscriptionId = User.GiveUserId()
                }
            });
            if (cardList.ResponseMessage.ErrorCode != 0)
            {
                ViewBag.ServiceError = CMS.Localization.Common.PaymentWithCardNotAvailable;
                return View();
            }
            if (cardList.RegisteredCardList == null)
            {
                return RedirectToAction("Payment", new { id = id });
            }
            ViewBag.CardsList = cardList.RegisteredCardList == null ? Enumerable.Empty<object>() : cardList.RegisteredCardList.Select(c => new { CardToken = c.Token, MaskedCardNumber = c.MaskedCardNo }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentSelection(long? id, string token)
        {
            var payableAmount = GetPayableAmount(User.GiveUserId(), id);
            if (payableAmount == 0m)
                return RedirectToAction("PaymentSelection");
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var mobileExpressPayBill = client.MobilexpressPayBill(new CustomerServiceMobilexpressPayBillRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username,
                MobileExpressPayBillParameters = new MobilexpressPayBillRequest()
                {
                    BillId = id,
                    PayableAmount = payableAmount,
                    Token = token,
                    SubscriptionId = User.GiveUserId(),
                    HttpContextParameters = new HttpContextParameters()
                    {
                        UserAgent = Request.UserAgent,
                        UserHostAddress = Request.UserHostAddress
                    }
                }
            });
            if (mobileExpressPayBill.ResponseMessage.ErrorCode != 0)
            {
                generalLogger.Error($"Error calling 'Mobile express pay bill' from web service. Code : {mobileExpressPayBill.ResponseMessage.ErrorCode} ");
                return ReturnMessageUrl(Url.Action("PaymentSelection"), CMS.Localization.Errors.InternalErrorDescription, false);
            }
            return ReturnMessageUrl(Url.Action("BillsAndPayments"), mobileExpressPayBill.ResponseMessage.ErrorMessage, true);
        }

        [HttpGet]
        public ActionResult Payment(long? id)
        {
            var baseRequest = new GenericServiceSettings();
            var tokenKey = VPOSTokenManager.RegisterPaymentToken(new BillPaymentToken()
            {
                SubscriberId = User.GiveUserId().Value,
                BillID = id
            });
            if (GetPayableAmount(User.GiveUserId(), id) == 0m)
            {
                return RedirectToAction("BillsAndPayments");
            }
            {
                AuthController.SignoutUser(Request.GetOwinContext());
                AuthController.SignInCurrentUserAgain(Request.GetOwinContext());
            }
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var VPOSFormResponse = client.GetVPOSForm(new CustomerServiceVPOSFormRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username,
                VPOSFormParameters = new VPOSFormRequest()
                {
                    FailUrl = Url.Action("VPOSFail", null, new { id = tokenKey }, Request.Url.Scheme),
                    OkUrl = Url.Action("VPOSSuccess", null, new { id = tokenKey }, Request.Url.Scheme),
                    PayableAmount = GetPayableAmount(User.GiveUserId(), id),
                    SubscriptionId = User.GiveUserId()
                }
            });
            if (VPOSFormResponse.ResponseMessage.ErrorCode != 0)
            {
                return RedirectToAction("BillsAndPayments");
            }
            ViewBag.POSForm = VPOSFormResponse.VPOSFormResponse.HtmlForm;
            return View(viewName: "3DHostPayment");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult VPOSSuccess(string id)
        {
            var paymentToken = VPOSTokenManager.RetrievePaymentToken(id);
            if (paymentToken == null)
            {
                Session["POSErrorMessage"] = CMS.Localization.Common.InvalidTokenKey;
                return RedirectToAction("BillsAndPayments");
            }
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var dbSubscription = client.SubscriptionBasicInfo(new CustomerServiceBaseRequest()
            {
                Username = baseRequest.Username,
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = paymentToken.SubscriberId
                }
            });
            if (dbSubscription.ResponseMessage.ErrorCode != 0)
            {
                Session["POSErrorMessage"] = CMS.Localization.Common.SubscriberNotFound;
                return RedirectToAction("BillsAndPayments");
            }
            //----------- bill paymnet ------------
            if (paymentToken is BillPaymentToken)
            {
                var billPaymentToken = (BillPaymentToken)paymentToken;

                var payableAmount = GetPayableAmount(dbSubscription.SubscriptionBasicInformationResponse.ID, billPaymentToken.BillID);
                // billed sub
                if (dbSubscription.SubscriptionBasicInformationResponse.HasBilling)
                {
                    var billBaseRequest = new GenericServiceSettings();
                    var dbBills = client.GetCustomerBills(new CustomerServiceBaseRequest()
                    {
                        Culture = billBaseRequest.Culture,
                        Hash = billBaseRequest.Hash,
                        Rand = billBaseRequest.Rand,
                        Username = billBaseRequest.Username,
                        SubscriptionParameters = new BaseSubscriptionRequest()
                        {
                            SubscriptionId = paymentToken.SubscriberId
                        }
                    });
                    if (dbBills.ResponseMessage.ErrorCode != 0)
                    {
                        Session["POSErrorMessage"] = dbBills.ResponseMessage.ErrorMessage;
                        return RedirectToAction("BillsAndPayments");
                    }
                    var billIds = billPaymentToken.BillID.HasValue ? new[] { billPaymentToken.BillID.Value } : dbBills.GetCustomerBillsResponse.CustomerBills.Where(b => b.Status == 1).Select(b => b.ID).ToArray();
                    paymentLogger.Debug("Received successfull payment for clientId= {1}, billId= {2} with total of {3}:" + Environment.NewLine + "{0}",
                                            string.Join(Environment.NewLine, Request.Form.AllKeys.Select(key => key + ": " + Request.Form[key])),
                                            dbSubscription.SubscriptionBasicInformationResponse.SubscriberNo,
                                            billPaymentToken.BillID.HasValue ? billPaymentToken.BillID.Value.ToString() : string.Join(",", billIds),
                                            payableAmount.ToString());
                    var unpaidBills = dbBills.GetCustomerBillsResponse.CustomerBills.Where(bill => bill.Status == 1).ToList(); // 1 = unpaid enum
                    if (billPaymentToken.BillID.HasValue)
                        unpaidBills = unpaidBills.Where(b => b.ID == billPaymentToken.BillID).ToList();

                    var payBills = PayBills(unpaidBills.Select(bill => bill.ID).ToArray(), (short)CMS.Localization.Enums.SubscriptionPaidType.Billing,
                        dbSubscription.SubscriptionBasicInformationResponse.ID, (int)CMS.Localization.Enums.PaymentType.VirtualPos, (int)CMS.Localization.Enums.AccountantType.Seller);
                    if (payBills.ResponseMessage.ErrorCode != 0)
                    {
                        Session["POSErrorMessage"] = payBills.ResponseMessage.ErrorMessage;
                        return RedirectToAction("BillsAndPayments");
                    }
                    var smsBaseRequest = new GenericServiceSettings();
                    var SendSubscriberSMS = client.SendSubscriberSMS(new CustomerServiceSendSubscriberSMSRequest()
                    {
                        Culture = smsBaseRequest.Culture,
                        Username = smsBaseRequest.Username,
                        Hash = smsBaseRequest.Hash,
                        Rand = smsBaseRequest.Rand,
                        SendSubscriberSMS = new SendSubscriberSMSRequest()
                        {
                            BillIds = billIds,
                            PayableAmount = payableAmount,
                            SubscriptionId = dbSubscription.SubscriptionBasicInformationResponse.ID,
                            SubscriptionPaidType = 1
                        }
                    });
                    if (SendSubscriberSMS.ResponseMessage.ErrorCode != 0)
                    {
                        Session["POSSuccessMessage"] = SendSubscriberSMS.ResponseMessage.ErrorMessage;
                        return RedirectToAction("BillsAndPayments");
                    }
                }
                //pre paid sub
                else
                {
                    paymentLogger.Debug("Received successfull payment for clientId= {1} with total of {2}:" + Environment.NewLine + "{0}",
                                            string.Join(Environment.NewLine, Request.Form.AllKeys.Select(key => key + ": " + Request.Form[key])),
                                            dbSubscription.SubscriptionBasicInformationResponse.SubscriberNo,
                                            payableAmount.ToString());

                    var payBills = PayBills(null, (short)CMS.Localization.Enums.SubscriptionPaidType.PrePaid,
                        dbSubscription.SubscriptionBasicInformationResponse.ID, (int)CMS.Localization.Enums.PaymentType.VirtualPos, (int)CMS.Localization.Enums.AccountantType.Admin);
                    if (payBills.ResponseMessage.ErrorCode != 0)
                    {
                        Session["POSErrorMessage"] = payBills.ResponseMessage.ErrorMessage;
                        return RedirectToAction("BillsAndPayments");
                    }
                    var smsBaseRequest = new GenericServiceSettings();
                    var SendSubscriberSMS = client.SendSubscriberSMS(new CustomerServiceSendSubscriberSMSRequest()
                    {
                        Culture = smsBaseRequest.Culture,
                        Username = smsBaseRequest.Username,
                        Hash = smsBaseRequest.Hash,
                        Rand = smsBaseRequest.Rand,
                        SendSubscriberSMS = new SendSubscriberSMSRequest()
                        {
                            SubscriptionPaidType = 2,
                            BillIds = null,
                            PayableAmount = payableAmount,
                            SubscriptionId = dbSubscription.SubscriptionBasicInformationResponse.ID
                        }
                    });
                    if (SendSubscriberSMS.ResponseMessage.ErrorCode != 0)
                    {
                        Session["POSSuccessMessage"] = SendSubscriberSMS.ResponseMessage.ErrorMessage;
                        return RedirectToAction("BillsAndPayments");
                    }
                }
            }
            //------------ quota sale ---------------
            else if (paymentToken is QuotaSaleToken)
            {
                var quotaSaleToken = (QuotaSaleToken)paymentToken;
                var quotaBaseRequest = new GenericServiceSettings();
                var quotaSale = client.QuotaSale(new CustomerServiceQuotaSaleRequest()
                {
                    Culture = quotaBaseRequest.Culture,
                    Hash = quotaBaseRequest.Hash,
                    Username = quotaBaseRequest.Username,
                    Rand = quotaBaseRequest.Rand,
                    QuotaSaleParameters = new QuotaSaleRequest()
                    {
                        SubscriptionId = dbSubscription.SubscriptionBasicInformationResponse.ID,
                        PackageId = quotaSaleToken.PackageID
                    }
                });
                if (quotaSale.ResponseMessage.ErrorCode != 0)
                {
                    Session["POSSuccessMessage"] = quotaSale.ResponseMessage.ErrorMessage;
                    return RedirectToAction("BillsAndPayments");
                }
            }

            Session["POSSuccessMessage"] = CMS.Localization.Common.POSSuccessMessage;
            return RedirectToAction("BillsAndPayments");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult VPOSFail(string id)
        {
            var paymentToken = VPOSTokenManager.RetrievePaymentToken(id);
            if (paymentToken == null)
            {
                Session["POSErrorMessage"] = CMS.Localization.Common.InvalidTokenKey;
                return RedirectToAction("BillsAndPayments");
            }
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var subscriptionBaseRequest = new GenericServiceSettings();
            var subscription = client.SubscriptionBasicInfo(new CustomerServiceBaseRequest()
            {
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = paymentToken.SubscriberId,
                },
                Culture = subscriptionBaseRequest.Culture,
                Hash = subscriptionBaseRequest.Hash,
                Rand = subscriptionBaseRequest.Rand,
                Username = subscriptionBaseRequest.Username
            });
            if (subscription.ResponseMessage.ErrorCode != 0)
            {
                Session["POSErrorMessage"] = subscription.ResponseMessage.ErrorMessage;
                return RedirectToAction("BillsAndPayments");
            }
            //--------- bill payment ---------
            if (paymentToken is BillPaymentToken)
            {
                var billPaymentToken = (BillPaymentToken)paymentToken;
                var billBaseRequest = new GenericServiceSettings();
                var dbBills = client.GetCustomerBills(new CustomerServiceBaseRequest()
                {
                    Culture = billBaseRequest.Culture,
                    Hash = billBaseRequest.Hash,
                    Rand = billBaseRequest.Rand,
                    Username = billBaseRequest.Username,
                    SubscriptionParameters = new BaseSubscriptionRequest()
                    {
                        SubscriptionId = paymentToken.SubscriberId
                    }
                });
                if (dbBills.ResponseMessage.ErrorCode != 0)
                {
                    Session["POSErrorMessage"] = dbBills.ResponseMessage.ErrorMessage;
                    return RedirectToAction("BillsAndPayments");
                }
                var unpaidBills = dbBills.GetCustomerBillsResponse.CustomerBills.Where(bill => bill.Status == 1).ToList(); // unpaid billstatus enum
                if (billPaymentToken.BillID.HasValue)
                {
                    unpaidBills = unpaidBills.Where(bill => bill.ID == billPaymentToken.BillID).ToList();
                }
                var clientCredits = dbBills.GetCustomerBillsResponse.SubscriptionCredits;
                var amount = unpaidBills.Sum(bill => bill.Total) - clientCredits;
                if (!subscription.SubscriptionBasicInformationResponse.HasBilling)
                {
                    amount = subscription.SubscriptionBasicInformationResponse.SubscriptionService.Price.Value;
                }
                // make changes to database
                unpaidLogger.Debug("Unsuccessfull payment for clientId= {1}, billId= {2} with total of {3}:" + Environment.NewLine + "{0}",
                    string.Join(Environment.NewLine, Request.Form.AllKeys.Select(key => key + ": " + Request.Form[key])),
                    subscription.SubscriptionBasicInformationResponse.SubscriberNo,
                    billPaymentToken.BillID.HasValue ? billPaymentToken.BillID.Value.ToString() : string.Join(",", unpaidBills.Select(bill => bill.ID.ToString())),
                    amount.ToString());
            }
            //------- quota sale -------------
            else if (paymentToken is QuotaSaleToken)
            {
                var quotaSaleToken = (QuotaSaleToken)paymentToken;
                var quotaBaseRequest = new GenericServiceSettings();
                var dbQuota = client.QuotaPackageList(new CustomerServiceQuotaPackagesRequest()
                {
                    Culture = quotaBaseRequest.Culture,
                    Hash = quotaBaseRequest.Hash,
                    Rand = quotaBaseRequest.Rand,
                    Username = quotaBaseRequest.Username
                });
                if (dbQuota.ResponseMessage.ErrorCode != 0)
                {
                    unpaidLogger.Debug("Unsuccessfull payment for clientId= {1} with total of {2}:" + Environment.NewLine + "{0}",
                                            string.Join(Environment.NewLine, Request.Form.AllKeys.Select(key => key + ": " + Request.Form[key])),
                                            subscription.SubscriptionBasicInformationResponse.SubscriberNo,
                                            dbQuota.ResponseMessage.ErrorMessage);
                }
                var dbQuotaPrice = dbQuota.QuotaPackageListResponse.Where(q => q.ID.ToString() == id).FirstOrDefault();
                unpaidLogger.Debug("Unsuccessfull payment for clientId= {1} with total of {2}:" + Environment.NewLine + "{0}",
                        string.Join(Environment.NewLine, Request.Form.AllKeys.Select(key => key + ": " + Request.Form[key])),
                        subscription.SubscriptionBasicInformationResponse.SubscriberNo,
                        dbQuotaPrice == null ? "-" : dbQuotaPrice.Price.ToString());
            }
            Session["POSErrorMessage"] = GetErrorMessage();
            return RedirectToAction("BillsAndPayments");
        }
        [ValidateAntiForgeryToken]
        public ActionResult EArchivePDF(long id)
        {
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var response = client.EArchivePDF(new CustomerServiceEArchivePDFRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Username = baseRequest.Username,
                Rand = baseRequest.Rand,
                EArchivePDFParameters = new EArchivePDFRequest()
                {
                    BillId = id,
                    SubscriptionId = User.GiveUserId()
                }
            });
            if (response.ResponseMessage.ErrorCode != 0)
            {
                return RedirectToAction("BillsAndPayments");
            }
            return File(response.EArchivePDFResponse.FileContent, response.EArchivePDFResponse.ContentType, response.EArchivePDFResponse.FileDownloadName);

        }
        [ValidateAntiForgeryToken]
        public ActionResult EArchivePDFMail(long id)
        {
            var response = new ServiceUtilities().EArchivePDFMail(id, User.GiveUserId());
            if (response.ResponseMessage.ErrorCode != 0)
            {
                generalLogger.Error(response.ResponseMessage.ErrorMessage);
                return ReturnMessageUrl(Url.Action("BillsAndPayments", "Payment"), response.ResponseMessage.ErrorMessage, false);
            }
            return ReturnMessageUrl(Url.Action("BillsAndPayments", "Payment"), response.ResponseMessage.ErrorMessage, true);
        }

        //public ActionResult RegisteredCards()
        //{
        //    return View();
        //}

        #region
        private CustomerServiceGenericAppSettingsResponse GenericAppSettings()
        {
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var settings = client.GenericAppSettings(new CustomerServiceGenericAppSettingsRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username
            });
            return settings;
        }
        private string GetErrorMessage()
        {
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var getVPOSError = client.GetVPOSErrorParameterName(new CustomerServiceVPOSErrorParameterNameRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username
            });
            return Request.Form[getVPOSError.ResponseMessage.ErrorCode != 0 ? string.Empty : getVPOSError.VPOSErrorParameterName];
        }
        private CustomerServicePayBillsResponse PayBills(long[] billIds, short? subscriptionPaidType, long? subscriptionId, int? paymentType, int? accountantType)
        {
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var payBillBaseRequest = new GenericServiceSettings();
            var payBills = client.PayBills(new CustomerServicePayBillsRequest()
            {
                Culture = payBillBaseRequest.Culture,
                Hash = payBillBaseRequest.Hash,
                Rand = payBillBaseRequest.Rand,
                Username = payBillBaseRequest.Username,
                PayBillsParameters = new PayBillsRequest()
                {
                    BillIds = billIds,
                    SubscriptionPaidType = subscriptionPaidType,
                    SubscriptionId = subscriptionId,
                    PaymentType = paymentType,
                    AccountantType = accountantType
                }
            });
            return payBills;
        }
        private decimal GetPayableAmount(long? subscriptionId, long? billId)
        {
            GenericCustomerServiceClient client = new GenericCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var response = client.BillPayableAmount(new CustomerServiceBillPayableAmountRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Username = baseRequest.Username,
                Rand = baseRequest.Rand,
                BillPayableAmountParameters = new BillPayableAmountRequest()
                {
                    SubscriptionId = subscriptionId,
                    BillId = billId
                }
            });
            if (response.ResponseMessage.ErrorCode != 0)
            {
                return 0m;
            }
            return response.PayableAmount ?? 0m;
        }
        #endregion

    }
}