﻿using CMS.ViewModels.Home;
using CMS.ViewModels.Supports;
//using CustomerManagementSystem.GenericCustomerServiceReference;
using MasterISS.CustomerService.NetspeedCustomerServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem
{
    public class ServiceUtilities
    {
        public CustomerServiceSubscriptionBasicInformationResponse GetSubscriptionInfo(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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

        }
        public CustomerServiceGetCustomerBillsResponse GetCustomerBillList(long? subscriptionId)
        {
            var billBaseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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

        }
        public CustomerServiceGetCustomerInfoResponse GetCustomerInfo(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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

        }
        public CustomerServiceGetCustomerTariffAndTrafficInfoResponse GetTariffAndTrafficInfo(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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

        }
        public CustomerServiceConnectionStatusResponse ConnectionStatus(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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
        public CustomerServiceChangeSubClientResponse ChangeSubClient(long? currentSubId, long? targetSubId)
        {
            var subBaseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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

        }
        public CustomerServiceGetCustomerSpecialOffersResponse CustomerSpecialOffers(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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

        }
        public CustomerServiceGetCustomerSupportListResponse GetSupportRequests(long? id)
        {
            var subBaseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var getSupportlist = client.GetSupportList(new CustomerServiceGetSupportListRequest()
                {
                    Culture = subBaseRequest.Culture,
                    Hash = subBaseRequest.Hash,
                    Rand = subBaseRequest.Rand,
                    Username = subBaseRequest.Username,
                    GetSupportList = new GetSupportListRequest()
                    {
                        SubscriptionId = id
                    }
                });
                return getSupportlist;
            }

        }
        public CustomerServiceSupportDetailMessagesResponse GetSupportDetails(long? subscriptionId, long? supportId)
        {
            var subBaseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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

        }
        public CustomerServiceHasActiveRequestResponse HasOpenRequest(long? subscriptionId)
        {
            var hasActiveBase = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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

        }
        public SelectList GetSupportTypes(object selectedValue = null)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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
                return new SelectList(response.ValueNamePairList.Select(pair => new { Name = pair.Name, Value = pair.Value }).OrderBy(pair => pair.Name), "Value", "Name", selectedValue);
            }

        }
        public SelectList GetSupportSubTypes(int? SupportTypeId, object selectedValue = null)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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
                return new SelectList(response.ValueNamePairList.Select(pair => new { Name = pair.Name, Value = pair.Value }).OrderBy(pair => pair.Name), "Value", "Name", selectedValue);
            }

        }
        public CustomerServiceSupportRegisterResponse SupportRegister(CMS.ViewModels.Supports.NewRequestViewModel request,List<Attachment> attachments, long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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
                        SubscriptionId = subscriptionId,
                        Attachments = attachments.ToArray()
                    },
                    Username = baseRequest.Username
                });
                return register;
            }

        }
        public CustomerServiceSendSupportMessageResponse SendSupportMessage(RequestSupportMessage requestMessage, List<Attachment> attachments, long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var newMessageResponse = client.SendSupportMessage(new CustomerServiceSendSupportMessageRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    SendSupportMessageParameters = new SendSupportMessageRequest()
                    {
                        Attachments = attachments.ToArray(),
                        Message = requestMessage.Message,
                        SubscriptionId = subscriptionId,
                        SupportId = requestMessage.ID,
                        SupportMessageType = requestMessage.ForAddNote == true ? (int)SupportMesssageTypes.AddNote
                                    : requestMessage.IsSolved == true ? (int)SupportMesssageTypes.ProblemSolved : (int)SupportMesssageTypes.OpenRequestAgain
                    }
                });
                return newMessageResponse;
            }

        }
        public CustomerServiceQuotaPackagesResponse QuotaPackageList()
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var QuotaListResponse = client.QuotaPackageList(new CustomerServiceQuotaPackagesRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Username = baseRequest.Username,
                    Rand = baseRequest.Rand
                });
                return QuotaListResponse;
            }

        }
        public CustomerServicePaymentTypeListResponse PaymentTypeList()
        {
            var paymentRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
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
        public CustomerServiceSupportStatusResponse SupportStatus(long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var statusResponse = client.SupportStatus(new CustomerServiceBaseRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    SubscriptionParameters = new BaseSubscriptionRequest()
                    {
                        SubscriptionId = subscriptionId
                    }
                });
                return statusResponse;
            }

        }
        public CustomerServiceExistingCustomerRegisterResponse SubscriptionRegister(SubcriptionRegisterViewModel register, long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                int? postalCode = null;
                if (!string.IsNullOrEmpty(register.SetupAddress.PostalCode))
                {
                    postalCode = Convert.ToInt32(register.SetupAddress.PostalCode);
                }
                var tariffs = new ServiceUtilities().GetTariffList();
                var selectedTariff = tariffs.ResponseMessage.ErrorCode != 0 ? null
                    : tariffs.ExternalTariffList == null ? null
                    : tariffs.ExternalTariffList.Count() == 0 ? null
                    : tariffs.ExternalTariffList.Where(t => t.TariffID == register.ServiceID).FirstOrDefault();
                var response = client.ExistingCustomerRegister(new CustomerServiceExistingCustomerRegisterRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    ExistingCustomerRegister = new ExistingCustomerRegisterRequest()
                    {
                        ExtraInfo = new MasterISS.CustomerService.NetspeedCustomerServiceReference.ExtraInfo()
                        {
                            ApplicationType = register.ExtraInfo.ApplicationType,
                            PSTN = register.ExtraInfo.PSTN,
                            XDSLNo = register.ExtraInfo.ServiceNo
                        },
                        SubscriberID = subscriptionId,
                        RegistrationInfo = new RegistrationInfo()
                        {
                            ServiceID = selectedTariff?.TariffID,
                            ReferralDiscount = register.ReferralDiscount == null ? null : new MasterISS.CustomerService.NetspeedCustomerServiceReference.ReferralDiscountInfo()
                            {
                                ReferenceNo = register.ReferralDiscount.ReferenceNo
                            },
                            SetupAddress = new MasterISS.CustomerService.NetspeedCustomerServiceReference.AddressInfo()
                            {
                                AddressNo = register.SetupAddress.AddressNo,
                                AddressText = register.SetupAddress.AddressText,
                                ApartmentID = register.SetupAddress.ApartmentID,
                                ApartmentNo = register.SetupAddress.ApartmentNo,
                                DistrictID = register.SetupAddress.DistrictID,
                                DistrictName = register.SetupAddress.DistrictName,
                                DoorID = register.SetupAddress.DoorID,
                                DoorNo = register.SetupAddress.DoorNo,
                                Floor = register.SetupAddress.Floor,
                                NeighbourhoodID = register.SetupAddress.NeighbourhoodID,
                                NeighbourhoodName = register.SetupAddress.NeighbourhoodName,
                                PostalCode = postalCode,
                                ProvinceID = register.SetupAddress.ProvinceID,
                                ProvinceName = register.SetupAddress.ProvinceName,
                                RuralCode = register.SetupAddress.RuralCode,
                                StreetID = register.SetupAddress.StreetID,
                                StreetName = register.SetupAddress.StreetName,
                            }
                        }
                    }
                });
                return response;
            }

        }
        public CustomerServiceNameValuePair1 GetProvinces()
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetProvinces(new CustomerServiceProvincesRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                });
                return result;
            }

        }
        public CustomerServiceNameValuePair1 GetProvinceDistricts(long? code)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetProvinceDistricts(new CustomerServiceNameValuePairRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    ItemCode = code
                });
                return result;
            }

        }
        public CustomerServiceNameValuePair1 GetDistrictRuralRegions(long? code)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetDistrictRuralRegions(new CustomerServiceNameValuePairRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    ItemCode = code
                });
                return result;
            }

        }
        public CustomerServiceNameValuePair1 GetRuralRegionNeighbourhoods(long? code)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetRuralRegionNeighbourhoods(new CustomerServiceNameValuePairRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    ItemCode = code
                });
                return result;

            }

        }
        public CustomerServiceNameValuePair1 GetNeighbourhoodStreets(long? code)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetNeighbourhoodStreets(new CustomerServiceNameValuePairRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    ItemCode = code
                });
                return result;
            }

        }
        public CustomerServiceNameValuePair1 GetStreetBuildings(long? code)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetStreetBuildings(new CustomerServiceNameValuePairRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    ItemCode = code
                });
                return result;
            }

        }
        public CustomerServiceNameValuePair1 GetBuildingApartments(long? code)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetBuildingApartments(new CustomerServiceNameValuePairRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    ItemCode = code
                });
                return result;
            }

        }
        public CustomerServiceAddressDetailsResponse GetApartmentAddress(long? code)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetApartmentAddress(new CustomerServiceAddressDetailsRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    BBK = code
                });
                return result;
            }

        }
        public CustomerServiceNameValuePair GetCommitmentLengths()
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.CommitmentLengthList(new CustomerServiceCommitmentLengthsRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                });
                return result;
            }

        }
        public CustomerServiceExternalTariffResponse GetTariffList()
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.ExternalTariffList(new CustomerServiceExternalTariffRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                });
                return result;
            }

        }
        public CustomerServiceServiceAvailabilityResponse ServiceAvailability(string bbk)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.ServiceAvailability(new CustomerServiceServiceAvailabilityRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    ServiceAvailabilityParameters = new ServiceAvailabilityRequest()
                    {
                        bbk = bbk
                    }
                });
                return result;
            }

        }
        public CustomerServiceGetCustomerFileResponse GetClientAttachmentList(long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetCustomerFiles(new CustomerServiceBaseRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    SubscriptionParameters = new BaseSubscriptionRequest()
                    {
                        SubscriptionId = subscriptionId
                    }
                });
                return result;
            }

        }
        public CustomerServiceGetClientAttachmentResponse GetClientAttachment(long? subscriptionId, string fileName)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetClientAttachment(new CustomerServiceGetClientAttachmentRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    GetClientAttachment = new GetClientAttachmentRequest()
                    {
                        FileName = fileName,
                        SubscriptionId = subscriptionId
                    }
                });
                return result;
            }

        }
        //public CustomerServiceSaveSupportAttachmentResponse SaveSupportAttachment(long stageId, string fileName, byte[] fileContent, string fileExtention, long? supportRequestId)
        //{
        //    var baseRequest = new GenericServiceSettings();
        //    using (var client = new NetspeedCustomerServiceClient())
        //    {
        //        var result = client.SaveSupportAttachment(new CustomerServiceSaveSupportAttachmentRequest()
        //        {
        //            Culture = baseRequest.Culture,
        //            Hash = baseRequest.Hash,
        //            Rand = baseRequest.Rand,
        //            Username = baseRequest.Username,
        //            SaveSupportAttachmentParameters = new SaveSupportAttachmentRequest()
        //            {
        //                FileContent = fileContent,
        //                FileExtention = fileExtention,
        //                FileName = fileName,
        //                StageId = stageId,
        //                SupportRequestId = supportRequestId
        //            }
        //        });
        //        return result;
        //    }

        //}
        public CustomerServicGetSupportAttachmentListResponse GetSupportAttachmentList(long? supportId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetSupportAttachmentList(new CustomerServiceGetSupportAttachmentListRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    GetSupportAttachmentsParameters = new GetSupportAttachmentListRequest()
                    {
                        RequestId = supportId
                    }
                });
                return result;
            }

        }
        public CustomerServiceGetSupportAttachmentResponse GetSupportAttachment(long? supportId, string fileName)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetSupportAttachment(new CustomerServiceGetSupportAttachmentRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    GetSupportAttachmentParameters = new GetSupportAttachmentRequest()
                    {
                        FileName = fileName,
                        SupportRequestId = supportId
                    }
                });
                return result;
            }

        }
        public CustomerServiceCustomerAuthenticationWithPasswordResponse AuthenticationWithPassword(string customerCode, string password)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.CustomerAuthenticationWithPassword(new CustomerServiceAuthenticationWithPasswordRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    AuthenticationWithPasswordParameters = new AuthenticationWithPasswordRequest()
                    {
                        CustomerCode = customerCode,
                        Password = password
                    }
                });
                return result;
            }

        }
        public CustomerServiceGenericAppSettingsResponse CustomerWebsiteGenericSettings()
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GenericAppSettings(new CustomerServiceGenericAppSettingsRequest()
                {
                    Username = baseRequest.Username,
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand
                });
                return result;
            }

        }
        public CustomerServiceHasClientPreRegisterResponse HasClientPreRegister(long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.HasClientPreRegisterSubscription(new CustomerServiceBaseRequest()
                {
                    Username = baseRequest.Username,
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    SubscriptionParameters = new BaseSubscriptionRequest()
                    {
                        SubscriptionId = subscriptionId
                    }
                });
                return result;
            }

        }
        public CustomerServiceSaveClientAttachmentResponse SaveClientAttachment(SaveClientAttachmentViewModel attachment)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.SaveClientAttachment(new CustomerServiceSaveClientAttachmentRequest()
                {
                    Username = baseRequest.Username,
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    SaveClientAttachmentParameters = new SaveClientAttachmentRequest()
                    {
                        AttachmentType = attachment.AttachmentType,
                        FileContent = attachment.FileContent,
                        FileExtention = attachment.FileExtention,
                        SubscriptionId = attachment.SubscriptionId
                    }
                });
                return result;
            }

        }
        public CustomerServiceGetClientPDFFormResponse GetClientPDFForm(long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetClientPDFForm(new CustomerServiceBaseRequest()
                {
                    Username = baseRequest.Username,
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    SubscriptionParameters = new BaseSubscriptionRequest()
                    {
                        SubscriptionId = subscriptionId
                    }
                });
                return result;
            }

        }
        public CustomerServiceEArchivePDFMailResponse EArchivePDFMail(long? billId, long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.EArchivePDFMail(new CustomerServiceEArchivePDFRequest()
                {
                    Username = baseRequest.Username,
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    EArchivePDFParameters = new EArchivePDFRequest()
                    {
                        BillId = billId,
                        SubscriptionId = subscriptionId
                    }
                });
                return result;
            }

        }
        //public CustomerServiceChangeClientInfoConfirmResponse ChangeClientInfoConfirm(long? subscriptionId, string phoneNo, string email)
        //{
        //    var baseRequest = new GenericServiceSettings();
        //    var result = client.ChangeClientInfoConfirm(new CustomerServiceChangeClientInfoConfirmRequest()
        //    {
        //        Username = baseRequest.Username,
        //        Culture = baseRequest.Culture,
        //        Hash = baseRequest.Hash,
        //        Rand = baseRequest.Rand,
        //        ChangeClientInfoConfirmRequest = new ChangeClientInfoConfirmRequest()
        //        {
        //            ContactPhoneNo = phoneNo,
        //            Email = email,
        //            SubscriptionId = subscriptionId
        //        }
        //    });
        //    return result;
        //}
        //public CustomerServiceChangeClientInfoResponse ChangeClientInfoSMSCheck(string phoneNo, long? subscriptionId)
        //{
        //    var baseRequest = new GenericServiceSettings();
        //    var result = client.ChangeClientInfoSMSCheck(new CustomerServiceChangeClientInfoRequest()
        //    {
        //        Username = baseRequest.Username,
        //        Culture = baseRequest.Culture,
        //        Hash = baseRequest.Hash,
        //        Rand = baseRequest.Rand,
        //        ChangeClientInfoRequest = new ChangeClientInfoRequest()
        //        {
        //            SubscriptionId = subscriptionId,
        //            ContactPhoneNo = phoneNo
        //        }
        //    });
        //    return result;
        //}
        public CustomerServiceChangeClientInfoConfirmResponse ChangeClientOnlinePassword(string onlinePassword, long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.ChangeClientOnlinePassword(new CustomerServiceChangeClientOnlinePasswordRequest()
                {
                    Username = baseRequest.Username,
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    ChangeClientOnlinePasswordParameters = new ChangeClientOnlinePasswordRequest()
                    {
                        OnlinePassword = onlinePassword,
                        SubscriptionId = subscriptionId
                    }
                });
                return result;
            }

        }
        public CustomerServiceSubscriberListResponse GetSubscriptionList(long? subscriptionId)
        {
            var baseRequest = new GenericServiceSettings();
            using (var client = new NetspeedCustomerServiceClient())
            {
                var result = client.GetSubscriptionList(new CustomerServiceBaseRequest()
                {
                    Username = baseRequest.Username,
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    SubscriptionParameters = new BaseSubscriptionRequest()
                    {
                        SubscriptionId = subscriptionId
                    }
                });
                return result;
            }

        }
    }
}