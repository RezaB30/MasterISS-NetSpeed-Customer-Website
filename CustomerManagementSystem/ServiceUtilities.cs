using CMS.ViewModels.Supports;
using CustomerManagementSystem.GenericCustomerServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem
{
    public class ServiceUtilities
    {
        GenericCustomerServiceReference.GenericCustomerServiceClient client = new GenericCustomerServiceReference.GenericCustomerServiceClient();
        public CustomerServiceSubscriptionBasicInformationResponse GetSubscriptionInfo(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            var dbSubscription = client.SubscriptionBasicInfo(new CustomerServiceBaseRequest()
            {
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = id
                },
                Culture = subBaseRequest.Culture,
                Hash = subBaseRequest.Hash,
                Rand = subBaseRequest.Rand,
                Username = subBaseRequest.Username
            });
            return dbSubscription;
        }
        public CustomerServiceGetCustomerBillsResponse GetCustomerBillList(long? subscriptionId)
        {
            var billBaseRequest = new GenericServiceSettings();
            var bills = client.GetCustomerBills(new CustomerServiceBaseRequest()
            {
                Culture = billBaseRequest.Culture,
                Hash = billBaseRequest.Hash,
                Rand = billBaseRequest.Rand,
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = subscriptionId,
                },
                Username = billBaseRequest.Username
            });
            return bills;
        }
        public CustomerServiceGetCustomerInfoResponse GetCustomerInfo(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            var customer = client.GetCustomerInfo(new CustomerServiceBaseRequest()
            {
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = id
                },
                Culture = subBaseRequest.Culture,
                Hash = subBaseRequest.Hash,
                Rand = subBaseRequest.Rand,
                Username = subBaseRequest.Username
            });
            return customer;
        }
        public CustomerServiceGetCustomerTariffAndTrafficInfoResponse GetTariffAndTrafficInfo(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            var tariffAndtraficInfo = client.GetCustomerTariffAndTrafficInfo(new CustomerServiceBaseRequest()
            {
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = id
                },
                Culture = subBaseRequest.Culture,
                Hash = subBaseRequest.Hash,
                Rand = subBaseRequest.Rand,
                Username = subBaseRequest.Username
            });
            return tariffAndtraficInfo;
        }
        public CustomerServiceConnectionStatusResponse ConnectionStatus(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            var connectionStatus = client.ConnectionStatus(new CustomerServiceBaseRequest()
            {
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = id
                },
                Culture = subBaseRequest.Culture,
                Hash = subBaseRequest.Hash,
                Rand = subBaseRequest.Rand,
                Username = subBaseRequest.Username
            });
            return connectionStatus;
        }
        public CustomerServiceChangeSubClientResponse ChangeSubClient(long? currentSubId, long? targetSubId)
        {
            var subBaseRequest = new GenericServiceSettings();
            var changeSubClient = client.ChangeSubClient(new CustomerServiceChangeSubClientRequest()
            {
                Culture = subBaseRequest.Culture,
                Hash = subBaseRequest.Hash,
                Rand = subBaseRequest.Rand,
                Username = subBaseRequest.Username,
                ChangeSubClientRequest = new ChangeSubClientRequest()
                {
                    CurrentSubscriptionID = currentSubId,
                    TargetSubscriptionID = targetSubId
                }
            });
            return changeSubClient;
        }
        public CustomerServiceGetCustomerSpecialOffersResponse CustomerSpecialOffers(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            var specialOffer = client.GetCustomerSpecialOffers(new CustomerServiceBaseRequest()
            {
                Culture = subBaseRequest.Culture,
                Hash = subBaseRequest.Hash,
                Rand = subBaseRequest.Rand,
                Username = subBaseRequest.Username,
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = id
                }
            });
            return specialOffer;
        }
        public CustomerServiceGetCustomerSupportListResponse GetSupportRequests(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            var getSupportlist = client.GetSupportList(new CustomerServiceBaseRequest()
            {
                Culture = subBaseRequest.Culture,
                Hash = subBaseRequest.Hash,
                Rand = subBaseRequest.Rand,
                Username = subBaseRequest.Username,
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = id
                }
            });
            return getSupportlist;
        }
        public CustomerServiceSupportDetailMessagesResponse GetSupportDetails(long? subscriptionId, long? supportId)
        {
            var subBaseRequest = new GenericServiceSettings();
            var getSupportDetails = client.GetSupportDetailMessages(new CustomerServiceSupportDetailMessagesRequest()
            {
                Culture = subBaseRequest.Culture,
                Hash = subBaseRequest.Hash,
                Rand = subBaseRequest.Rand,
                Username = subBaseRequest.Username,
                SupportDetailMessagesParameters = new SupportDetailMessagesRequest()
                {
                    SubscriptionId = subscriptionId,
                    SupportId = supportId
                }
            });
            return getSupportDetails;
        }
        public CustomerServiceHasActiveRequestResponse HasOpenRequest(long? subscriptionId)
        {
            var hasActiveBase = new GenericServiceSettings();

            var hasActiveRequest = client.SupportHasActiveRequest(new CustomerServiceBaseRequest()
            {
                Culture = hasActiveBase.Culture,
                Hash = hasActiveBase.Hash,
                Username = hasActiveBase.Username,
                Rand = hasActiveBase.Rand,
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = subscriptionId
                }
            });
            return hasActiveRequest;
        }
        public SelectList GetSupportTypes(object selectedValue = null)
        {
            var baseRequest = new GenericServiceSettings();
            var response = client.GetSupportTypes(new CustomerServiceSupportTypesRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username,
                SupportTypesParameters = new SupportTypesRequest()
                {
                    IsDisabled = false,
                    IsStaffOnly = false
                }
            });
            if (response.ResponseMessage.ErrorCode != 0)
            {
                return new SelectList(Enumerable.Empty<object>());
            }
            return new SelectList(response.NameValuePairList.Select(pair => new { Name = pair.Name, Value = pair.Value }), "Value", "Name", selectedValue);
        }
        public SelectList GetSupportSubTypes(int? SupportTypeId, object selectedValue = null)
        {
            var baseRequest = new GenericServiceSettings();
            var response = client.GetSupportSubTypes(new CustomerServiceSupportSubTypesRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username,
                SupportSubTypesParameters = new SupportSubTypesRequest()
                {
                    IsDisabled = false,
                    SupportTypeID = SupportTypeId
                }
            });
            if (response.ResponseMessage.ErrorCode != 0)
            {
                return new SelectList(Enumerable.Empty<object>());
            }
            return new SelectList(response.NameValuePairList.Select(pair => new { Name = pair.Name, Value = pair.Value }), "Value", "Name", selectedValue);
        }
        public CustomerServiceSupportRegisterResponse SupportRegister(CMS.ViewModels.Supports.NewRequestViewModel request, long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            var register = client.SupportRegister(new CustomerServiceSupportRegisterRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                SupportRegisterParameters = new SupportRegisterRequest()
                {
                    Description = request.Description,
                    RequestTypeId = request.RequestTypeId,
                    SubRequestTypeId = request.SubRequestTypeId,
                    SubscriptionId = subscriptionId
                },
                Username = baseRequest.Username
            });
            return register;
        }
        public CustomerServiceSendSupportMessageResponse SendSupportMessage(RequestSupportMessage requestMessage, long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            var newMessageResponse = client.SendSupportMessage(new CustomerServiceSendSupportMessageRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Rand = baseRequest.Rand,
                Username = baseRequest.Username,
                SendSupportMessageParameters = new SendSupportMessageRequest()
                {
                    Message = requestMessage.Message,
                    SubscriptionId = subscriptionId,
                    SupportId = requestMessage.ID,
                    SupportMessageType = requestMessage.ForAddNote == true ? (int)SupportMesssageTypes.AddNote
                    : requestMessage.IsSolved == true ? (int)SupportMesssageTypes.ProblemSolved : (int)SupportMesssageTypes.OpenRequestAgain
                }
            });
            return newMessageResponse;
        }
        public CustomerServiceQuotaPackagesResponse QuotaPackageList()
        {
            var baseRequest = new GenericServiceSettings();
            var QuotaListResponse = client.QuotaPackageList(new CustomerServiceQuotaPackagesRequest()
            {
                Culture = baseRequest.Culture,
                Hash = baseRequest.Hash,
                Username = baseRequest.Username,
                Rand = baseRequest.Rand
            });
            return QuotaListResponse;
        }
        public CustomerServicePaymentTypeListResponse PaymentTypeList()
        {
            var paymentRequest = new GenericServiceSettings();
            var paymentTypes = client.PaymentTypeList(new CustomerServicePaymentTypeListRequest()
            {
                Culture = paymentRequest.Culture,
                Hash = paymentRequest.Hash,
                Rand = paymentRequest.Rand,
                Username = paymentRequest.Username
            });
            return paymentTypes;
        }
    }
}