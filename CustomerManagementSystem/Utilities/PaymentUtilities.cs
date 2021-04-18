using CustomerManagementSystem.Controllers;
using CustomerManagementSystem.VPOSToken;
using MasterISS.CustomerService.NetspeedCustomerServiceReference;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerManagementSystem.Utilities
{

    public static class PaymentUtilities
    {
        static Logger paymentLogger = LogManager.GetLogger("payments");
        static Logger unpaidLogger = LogManager.GetLogger("unpaid");
        public static bool VPOSSuccess(string id)
        {
            var paymentToken = VPOSTokenManager.RetrievePaymentToken(id);
            if (paymentToken == null)
            {

                HttpContext.Current.Session["POSErrorMessage"] = CMS.Localization.Common.InvalidTokenKey;
                return false;
                //return RedirectToAction("BillsAndPayments");
            }
            NetspeedCustomerServiceClient client = new NetspeedCustomerServiceClient();
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
                HttpContext.Current.Session["POSErrorMessage"] = CMS.Localization.Common.SubscriberNotFound;
                return false;
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
                        HttpContext.Current.Session["POSErrorMessage"] = dbBills.ResponseMessage.ErrorMessage;
                        return false;
                    }
                    var billIds = billPaymentToken.BillID.HasValue ? new[] { billPaymentToken.BillID.Value } : dbBills.GetCustomerBillsResponse.CustomerBills.Where(b => b.Status == 1).Select(b => b.ID).ToArray();
                    paymentLogger.Debug("Received successfull payment for clientId= {1}, billId= {2} with total of {3}:" + Environment.NewLine + "{0}",
                                            string.Join(Environment.NewLine, HttpContext.Current.Request.Form.AllKeys.Select(key => key + ": " + HttpContext.Current.Request.Form[key])),
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
                        HttpContext.Current.Session["POSErrorMessage"] = payBills.ResponseMessage.ErrorMessage;
                        return false;
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
                        HttpContext.Current.Session["POSSuccessMessage"] = SendSubscriberSMS.ResponseMessage.ErrorMessage;
                        return false;
                    }
                }
                //pre paid sub
                else
                {
                    paymentLogger.Debug("Received successfull payment for clientId= {1} with total of {2}:" + Environment.NewLine + "{0}",
                                            string.Join(Environment.NewLine, HttpContext.Current.Request.Form.AllKeys.Select(key => key + ": " + HttpContext.Current.Request.Form[key])),
                                            dbSubscription.SubscriptionBasicInformationResponse.SubscriberNo,
                                            payableAmount.ToString());

                    var payBills = PayBills(null, (short)CMS.Localization.Enums.SubscriptionPaidType.PrePaid,
                        dbSubscription.SubscriptionBasicInformationResponse.ID, (int)CMS.Localization.Enums.PaymentType.VirtualPos, (int)CMS.Localization.Enums.AccountantType.Admin);
                    if (payBills.ResponseMessage.ErrorCode != 0)
                    {
                        HttpContext.Current.Session["POSErrorMessage"] = payBills.ResponseMessage.ErrorMessage;
                        return false;
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
                        HttpContext.Current.Session["POSSuccessMessage"] = SendSubscriberSMS.ResponseMessage.ErrorMessage;
                        return false;
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
                    HttpContext.Current.Session["POSSuccessMessage"] = quotaSale.ResponseMessage.ErrorMessage;
                    return false;
                }
            }

            HttpContext.Current.Session["POSSuccessMessage"] = CMS.Localization.Common.POSSuccessMessage;
            return true;
        }
        public static bool VPOSFail(string id)
        {
            var paymentToken = VPOSTokenManager.RetrievePaymentToken(id);
            if (paymentToken == null)
            {
                HttpContext.Current.Session["POSErrorMessage"] = CMS.Localization.Common.InvalidTokenKey;
                return false;
            }
            NetspeedCustomerServiceClient client = new NetspeedCustomerServiceClient();
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
                HttpContext.Current.Session["POSErrorMessage"] = subscription.ResponseMessage.ErrorMessage;
                return false;                
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
                    HttpContext.Current.Session["POSErrorMessage"] = dbBills.ResponseMessage.ErrorMessage;                    
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
                    string.Join(Environment.NewLine, HttpContext.Current.Request.Form.AllKeys.Select(key => key + ": " + HttpContext.Current.Request.Form[key])),
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
                                            string.Join(Environment.NewLine, HttpContext.Current.Request.Form.AllKeys.Select(key => key + ": " + HttpContext.Current.Request.Form[key])),
                                            subscription.SubscriptionBasicInformationResponse.SubscriberNo,
                                            dbQuota.ResponseMessage.ErrorMessage);
                }
                var dbQuotaPrice = dbQuota.QuotaPackageListResponse.Where(q => q.ID.ToString() == id).FirstOrDefault();
                unpaidLogger.Debug("Unsuccessfull payment for clientId= {1} with total of {2}:" + Environment.NewLine + "{0}",
                        string.Join(Environment.NewLine, HttpContext.Current.Request.Form.AllKeys.Select(key => key + ": " + HttpContext.Current.Request.Form[key])),
                        subscription.SubscriptionBasicInformationResponse.SubscriberNo,
                        dbQuotaPrice == null ? "-" : dbQuotaPrice.Price.ToString());
            }
            HttpContext.Current.Session["POSErrorMessage"] = GetErrorMessage();
            return true;
        }
        public static PaymentUtilitiesResponse Payment(long? id, long subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            var tokenKey = VPOSTokenManager.RegisterPaymentToken(new BillPaymentToken()
            {
                SubscriberId = subscriptionId,
                BillID = id
            });
            if (GetPayableAmount(subscriptionId, id) == 0m)
            {
                return new PaymentUtilitiesResponse()
                {
                    IsSuccess = false,
                    ResponseMessage = CMS.Localization.Common.NoHaveUnpaidBills
                };
            }
            {
                AuthController.SignoutUser(HttpContext.Current.GetOwinContext());
                AuthController.SignInCurrentUserAgain(HttpContext.Current.GetOwinContext());
            }
            var controller = HttpContext.Current.Request.RequestContext.RouteData.Values.Where(r => r.Key == "controller").Select(r => r.Value).FirstOrDefault();
            NetspeedCustomerServiceClient client = new NetspeedCustomerServiceClient();
            var requestContext = HttpContext.Current.Request.RequestContext;
            var VPOSFormResponse = client.GetVPOSForm(new CustomerServiceVPOSFormRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username,
                VPOSFormParameters = new VPOSFormRequest()
                {
                    FailUrl = new System.Web.Mvc.UrlHelper(requestContext).Action("VPOSFail", controller.ToString(), new { id = tokenKey }, HttpContext.Current.Request.Url.Scheme),
                    OkUrl = new System.Web.Mvc.UrlHelper(requestContext).Action("VPOSSuccess", controller.ToString(), new { id = tokenKey }, HttpContext.Current.Request.Url.Scheme),
                    PayableAmount = GetPayableAmount(subscriptionId, id),
                    SubscriptionId = subscriptionId
                }
            });
            if (VPOSFormResponse.ResponseMessage.ErrorCode != 0)
            {
                return new PaymentUtilitiesResponse()
                {
                    IsSuccess = false,
                    ResponseMessage = VPOSFormResponse.ResponseMessage.ErrorMessage
                };
            }
            return new PaymentUtilitiesResponse()
            {
                IsSuccess = true,
                HtmlForm = VPOSFormResponse.VPOSFormResponse.HtmlForm
            };
        }
        private static CustomerServicePayBillsResponse PayBills(long[] billIds, short? subscriptionPaidType, long? subscriptionId, int? paymentType, int? accountantType)
        {
            NetspeedCustomerServiceClient client = new NetspeedCustomerServiceClient();
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
        private static decimal GetPayableAmount(long? subscriptionId, long? billId)
        {
            NetspeedCustomerServiceClient client = new NetspeedCustomerServiceClient();
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
        private static string GetErrorMessage()
        {
            NetspeedCustomerServiceClient client = new NetspeedCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var getVPOSError = client.GetVPOSErrorParameterName(new CustomerServiceVPOSErrorParameterNameRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username
            });
            return HttpContext.Current.Request.Form[getVPOSError.ResponseMessage.ErrorCode != 0 ? string.Empty : getVPOSError.VPOSErrorParameterName];
        }
    }
    public class PaymentUtilitiesResponse
    {
        public bool IsSuccess { get; set; }
        public string ResponseMessage { get; set; }
        public string HtmlForm { get; set; }
    }
}