using MasterISS.CustomerService.NetspeedCustomerServiceReference;
//using CustomerManagementSystem.GenericCustomerServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerManagementSystem.Utilities
{
    public static class DomainCache
    {
        public static bool HasAnyTelekomDomains
        {
            get
            {
                NetspeedCustomerServiceClient client = new NetspeedCustomerServiceClient();
                var baseRequest = new GenericServiceSettings();
                var response = client.GenericAppSettings(new CustomerServiceGenericAppSettingsRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Username = baseRequest.Username,
                    Rand = baseRequest.Rand
                });
                if (response.ResponseMessage.ErrorCode != 0)
                {
                    return false;
                }
                return response.GenericAppSettings.HasAnyTelekomDomains;
            }
        }
    }
}