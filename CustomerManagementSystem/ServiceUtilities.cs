using CustomerManagementSystem.GenericCustomerServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}