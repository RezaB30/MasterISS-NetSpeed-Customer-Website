using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CMS.ViewModels.Home;
using NLog;
using RezaB.Web.Authentication;

namespace CustomerManagementSystem.Controllers
{
    public class HomeController : BaseController
    {
        Logger generalLogger = LogManager.GetLogger("general");
        public ActionResult Index()
        {
            if (User.IsSubscriptionNotActive())
                return RedirectToAction("RegisterTracking", "Home");
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
                //LineState = "Online",
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
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
        public ActionResult Documents() // belgelerim.html
        {
            var files = new ServiceUtilities().GetClientAttachmentList(User.GiveUserId());
            if (files.ResponseMessage.ErrorCode != 0 || files.CustomerFiles == null)
            {
                return View(Enumerable.Empty<CustomerDocumentsViewModel>());
            }
            var customerFiles = files.CustomerFiles.Select(f => new CustomerDocumentsViewModel()
            {
                FileExtention = f.FileExtention,
                FileName = f.FileInfo.Name,
                MIMEType = f.MIMEType,
                ServerSideName = f.ServerSideName
            }).ToArray();
            return View(customerFiles);
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public ActionResult DownloadAttachment(string fileName)
        {
            var clientAttachment = new ServiceUtilities().GetClientAttachment(User.GiveUserId(), fileName);
            if (clientAttachment.ResponseMessage.ErrorCode != 0)
            {
                if (clientAttachment.ResponseMessage.ErrorCode == 200 || clientAttachment.ResponseMessage.ErrorCode == 199)
                {
                    return ReturnMessageUrl(Url.Action("Documents", "Home"), CMS.Localization.Errors.InternalErrorDescription, false);
                }
                return ReturnMessageUrl(Url.Action("Documents", "Home"), clientAttachment.ResponseMessage.ErrorMessage, false);
            }
            return File(clientAttachment.GetClientAttachment.Content, clientAttachment.GetClientAttachment.MIMEType, clientAttachment.GetClientAttachment.FileInfo.Name);
        }
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
        public ActionResult MyServices() //hizmetlerim.html
        {
            return RedirectToAction("Index", "Home");
            //return View();
        }
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
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
            var subscriptionBag = Utilities.InternalUtilities.GetSubscriptionBag(User);
            var selectedSubscription = User.GiveUserId();
            var subscriptions = new SelectList(subscriptionBag.Select(s => new { Text = s.SubscriberNo, Value = s.ID }), "Value", "Text", selectedSubscription);
            return PartialView("~/Views/Shared/EditorPartials/Home/SubscriptionList.cshtml", new SubscriptionListVM() { SubscriptionList = subscriptions });
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
        [Authorize(Roles = "Customer")]
        public ActionResult GetAddressFromCode(string bbk)
        {
            var availability = new ServiceUtilities().ServiceAvailability(bbk);
            if (availability.ResponseMessage.ErrorCode != 0)
            {
                return Content(string.Format(CMS.Localization.Message.AddressDescription, "-"));
            }
            return Content(string.Format(CMS.Localization.Message.AddressDescription, availability.ServiceAvailabilityResponse.address));
        }
        [Authorize(Roles = "Customer")]
        public ActionResult LoadTariffs(string bbk)
        {
            var availability = new ServiceUtilities().ServiceAvailability(bbk);
            if (availability.ResponseMessage.ErrorCode != 0)
            {
                return Content(CMS.Localization.Errors.AvailabilityTariffNotFoundDescription);
            }
            var externalTariffs = new ServiceUtilities().GetTariffList();
            if (availability.ServiceAvailabilityResponse.FIBER.HasInfrastructureFiber)
            {
                var tariffList = externalTariffs.ExternalTariffList?.Where(t => t.HasFiber).Select(t => new CMS.ViewModels.Home.TariffsViewModel() { TariffName = t.DisplayName, TariffID = t.TariffID, Price = t.Price.ToString(), Speed = t.Speed });

                return PartialView("_TariffListPartial", tariffList);
            }
            if (availability.ServiceAvailabilityResponse.VDSL.HasInfrastructureVdsl || availability.ServiceAvailabilityResponse.ADSL.HasInfrastructureAdsl)
            {
                var tariffList = externalTariffs.ExternalTariffList?.Where(t => t.HasXDSL).Select(t => new CMS.ViewModels.Home.TariffsViewModel() { TariffName = t.DisplayName, TariffID = t.TariffID, Price = t.Price.ToString(), Speed = t.Speed });
                return PartialView("_TariffListPartial", tariffList);
            }
            return Content(CMS.Localization.Errors.AvailabilityTariffNotFoundDescription);
        }
        [Authorize(Roles = "Customer")]
        public ActionResult SubscriptionRegister()
        {
            var hasRegister = new ServiceUtilities().HasClientPreRegister(User.GiveUserId());
            if (hasRegister.HasClientPreRegister == null || hasRegister.HasClientPreRegister.HasPreRegister == true)
            {
                return ReturnMessageUrl(Url.Action("Index", "Home"), CMS.Localization.Errors.HasPreRegisterInProgress, false);
            }
            var provinces = new ServiceUtilities().GetProvinces();
            ViewBag.ProvinceList = new SelectList(provinces.ValueNamePairList ?? Enumerable.Empty<MasterISS.CustomerService.GenericCustomerServiceReference.ValueNamePair>(), "Value", "Name");
            var commitmentLengthList = new ServiceUtilities().GetCommitmentLengths();
            ViewBag.CommitmentLengthList = new SelectList(commitmentLengthList.ValueNamePairList ?? Enumerable.Empty<MasterISS.CustomerService.GenericCustomerServiceReference.ValueNamePair>(), "Value", "Name");
            return View();
        }
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SubscriptionRegister(SubcriptionRegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var hasRegister = new ServiceUtilities().HasClientPreRegister(User.GiveUserId());
                if (hasRegister.HasClientPreRegister == null || hasRegister.HasClientPreRegister.HasPreRegister == true)
                {
                    return ReturnMessageUrl(Url.Action("Index", "Home"), CMS.Localization.Errors.HasPreRegisterInProgress, false);
                }
                var address = new ServiceUtilities().GetApartmentAddress(register.SetupAddress.ApartmentID);
                if (address.ResponseMessage.ErrorCode == 0)
                {
                    var getAddress = address.AddressDetailsResponse;
                    var existingRegister = new SubcriptionRegisterViewModel()
                    {
                        SetupAddress = new AddressInfo()
                        {
                            AddressNo = getAddress.AddressNo,
                            AddressText = getAddress.AddressText,
                            ApartmentID = getAddress.ApartmentID,
                            ApartmentNo = getAddress.ApartmentNo,
                            DistrictID = getAddress.DistrictID,
                            DistrictName = getAddress.DistrictName,
                            DoorID = getAddress.DoorID,
                            DoorNo = getAddress.DoorNo,
                            Floor = register.SetupAddress.Floor,
                            NeighbourhoodID = getAddress.NeighbourhoodID,
                            NeighbourhoodName = getAddress.NeighbourhoodName,
                            PostalCode = register.SetupAddress.PostalCode,
                            ProvinceID = getAddress.ProvinceID,
                            ProvinceName = getAddress.ProvinceName,
                            RuralCode = getAddress.RuralCode,
                            StreetID = getAddress.StreetID,
                            StreetName = getAddress.StreetName
                        },
                        ServiceID = register.ServiceID,
                        ReferralDiscount = register.ReferralDiscount == null ? null : register.ReferralDiscount.ReferenceNo == null ? null : new ReferralDiscountInfo()
                        {
                            ReferenceNo = register.ReferralDiscount.ReferenceNo
                        }
                    };
                    var registerResponse = new ServiceUtilities().SubscriptionRegister(existingRegister, User.GiveUserId());
                    if (registerResponse.ResponseMessage.ErrorCode != 0)
                    {
                        if (registerResponse.KeyValuePairs != null && registerResponse.KeyValuePairs.Count() != 0 && registerResponse.KeyValuePairs.ContainsKey("ReferralDiscount.ReferenceNo"))
                        {
                            TempData["generic-error"] = CMS.Localization.Errors.InvalidReferenceNo;
                        }
                        else
                        {
                            TempData["generic-error"] = registerResponse.ResponseMessage.ErrorMessage;
                        }
                    }
                    else
                    {
                        TempData["generic-success"] = registerResponse.ResponseMessage.ErrorMessage;
                        return RedirectToAction("SubscriptionRegister");
                    }
                }
                else
                {
                    TempData["generic-error"] = address.ResponseMessage.ErrorMessage;
                }
            }
            var provinces = new ServiceUtilities().GetProvinces();
            var districts = new ServiceUtilities().GetProvinceDistricts(register.SetupAddress.ProvinceID);
            var rurals = new ServiceUtilities().GetDistrictRuralRegions(register.SetupAddress.DistrictID);
            var neighbourhoods = new ServiceUtilities().GetRuralRegionNeighbourhoods(register.SetupAddress.RuralCode);
            var streets = new ServiceUtilities().GetNeighbourhoodStreets(register.SetupAddress.NeighbourhoodID);
            var buildings = new ServiceUtilities().GetStreetBuildings(register.SetupAddress.StreetID);
            var apartments = new ServiceUtilities().GetBuildingApartments(register.SetupAddress.DoorID);
            ViewBag.ProvinceList = new SelectList(provinces.ValueNamePairList ?? Enumerable.Empty<MasterISS.CustomerService.GenericCustomerServiceReference.ValueNamePair>(), "Value", "Name", register.SetupAddress.ProvinceID);
            ViewBag.DistrictList = new SelectList(districts.ValueNamePairList ?? Enumerable.Empty<MasterISS.CustomerService.GenericCustomerServiceReference.ValueNamePair>(), "Value", "Name", register.SetupAddress.DistrictID);
            ViewBag.RuralList = new SelectList(rurals.ValueNamePairList ?? Enumerable.Empty<MasterISS.CustomerService.GenericCustomerServiceReference.ValueNamePair>(), "Value", "Name", register.SetupAddress.RuralCode);
            ViewBag.NeighbourhoodList = new SelectList(neighbourhoods.ValueNamePairList ?? Enumerable.Empty<MasterISS.CustomerService.GenericCustomerServiceReference.ValueNamePair>(), "Value", "Name", register.SetupAddress.NeighbourhoodID);
            ViewBag.StreetList = new SelectList(streets.ValueNamePairList ?? Enumerable.Empty<MasterISS.CustomerService.GenericCustomerServiceReference.ValueNamePair>(), "Value", "Name", register.SetupAddress.StreetID);
            ViewBag.BuildingList = new SelectList(buildings.ValueNamePairList ?? Enumerable.Empty<MasterISS.CustomerService.GenericCustomerServiceReference.ValueNamePair>(), "Value", "Name", register.SetupAddress.DoorID);
            ViewBag.ApartmentList = new SelectList(apartments.ValueNamePairList ?? Enumerable.Empty<MasterISS.CustomerService.GenericCustomerServiceReference.ValueNamePair>(), "Value", "Name", register.SetupAddress.ApartmentID);
            return View(register);
        }
        public ActionResult RegisterTracking()
        {
            var applicationRegisteredStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
            var checkingDocumentsStage = CMS.Localization.Enums.RegisterTrackingTypes.Passive;
            var activationStage = CMS.Localization.Enums.RegisterTrackingTypes.Passive;
            var connectionStage = CMS.Localization.Enums.RegisterTrackingTypes.Passive;
            var completedStage = CMS.Localization.Enums.RegisterTrackingTypes.Passive;
            var clientFiles = new ServiceUtilities().GetClientAttachmentList(User.GiveUserId());
            if (clientFiles.CustomerFiles != null && clientFiles.CustomerFiles.Where(cl => cl.FileInfo.Type == (int)CMS.Localization.Enums.ClientAttachmentTypes.IDCard).Any())
            {
                ViewBag.HasIDCardForm = true;
            }
            else
            {
                ViewBag.HasIDCardForm = false;
            }
            var customer = new ServiceUtilities().GetCustomerInfo(User.GiveUserId());
            if (customer.GetCustomerInfoResponse != null && customer.GetCustomerInfoResponse.CustomerState == (short)CMS.Localization.Enums.CustomerState.PreRegisterd)
            {
                applicationRegisteredStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
                checkingDocumentsStage = CMS.Localization.Enums.RegisterTrackingTypes.Waiting;
            }
            else if (customer.GetCustomerInfoResponse != null && customer.GetCustomerInfoResponse.CustomerState == (short)CMS.Localization.Enums.CustomerState.Registered)
            {
                applicationRegisteredStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
                checkingDocumentsStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
                activationStage = CMS.Localization.Enums.RegisterTrackingTypes.Waiting;
            }
            else if (customer.GetCustomerInfoResponse != null && customer.GetCustomerInfoResponse.CustomerState == (short)CMS.Localization.Enums.CustomerState.Reserved)
            {
                applicationRegisteredStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
                checkingDocumentsStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
                activationStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
                connectionStage = CMS.Localization.Enums.RegisterTrackingTypes.Waiting;
            }
            else
            {
                applicationRegisteredStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
                checkingDocumentsStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
                activationStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
                connectionStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
                completedStage = CMS.Localization.Enums.RegisterTrackingTypes.Success;
            }
            ViewBag.ApplicationRegisteredStage = applicationRegisteredStage;
            ViewBag.CheckingDocumentsStage = checkingDocumentsStage;
            ViewBag.ActivationStage = activationStage;
            ViewBag.ConnectionStage = connectionStage;
            ViewBag.CompletedStage = completedStage;
            return View();
        }
        public ActionResult GetPDFForm()
        {
            var pdfForm = new ServiceUtilities().GetClientPDFForm(User.GiveUserId());
            if (pdfForm.ResponseMessage.ErrorCode != 0)
            {
                return ReturnMessageUrl(Url.Action("Index", "Home"), pdfForm.ResponseMessage.ErrorMessage, false);
            }
            return File(pdfForm.GetClientPDFFormResult.FileContent, pdfForm.GetClientPDFFormResult.MIMEType, pdfForm.GetClientPDFFormResult.FileName);
        }
        [HttpPost]
        public ActionResult UploadCustomerFile(HttpPostedFileBase[] customerContract, HttpPostedFileBase[] customerDropFile, string[] selectedFiles)
        {
            if (customerContract == null || customerContract.Count() == 0 || selectedFiles == null || selectedFiles.Count() == 0)
            {
                return RedirectToAction("RegisterTracking", "Home");
            }
            var uploadFiles = customerContract.Where(c => selectedFiles.Contains(c.FileName)).ToList();
            var uplodDropFiles = customerDropFile.Where(c => selectedFiles.Contains(c.FileName)).ToArray();
            foreach (var item in uplodDropFiles)
            {
                uploadFiles.Add(item);
            }
            var successUploadFile = 0;
            var failUploadFile = 0;
            foreach (var file in uploadFiles)
            {
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    var attachmentByte = binaryReader.ReadBytes(file.ContentLength);
                    FileInfo fileInfo = new FileInfo(file.FileName);
                    var fileExtention = fileInfo.Extension.Replace(".", "");
                    var upload = new ServiceUtilities().SaveClientAttachment(new SaveClientAttachmentViewModel()
                    {
                        AttachmentType = (int)CMS.Localization.Enums.ClientAttachmentTypes.IDCard,
                        FileExtention = fileExtention,
                        SubscriptionId = User.GiveUserId(),
                        FileContent = attachmentByte
                    });
                    generalLogger.Info($"Upload client Attachment Service Response | Error Code : {upload.ResponseMessage.ErrorCode} - Error Message : {upload.ResponseMessage.ErrorMessage}");
                    if (upload.ResponseMessage.ErrorCode != 0)
                    {
                        failUploadFile++;
                    }
                    else
                    {
                        successUploadFile++;
                    }
                }

            }
            var failMessage = failUploadFile != 0 ? string.Format(CMS.Localization.Errors.UploadCustomerCardFail, failUploadFile) : null;
            var successMessage = successUploadFile != 0 ? string.Format(CMS.Localization.Errors.UploadCustomerCardSuccess, successUploadFile) : null;
            return ReturnMessageUrl(Url.Action("RegisterTracking", "Home"), $"{successMessage}{Environment.NewLine}{failMessage}", successMessage != null);
        }
        [Authorize(Roles = "Customer")]
        public ActionResult QuickSearch(string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    if (Session["quickSearch"] != null)
                    {
                        var sessionContent = Session["quickSearch"].ToString();
                        Session.Remove("quickSearch");
                        return Content(sessionContent);
                    }
                }
                var SearchList = new List<QuickSearch>();
                SearchList.Add(new CMS.ViewModels.Home.QuickSearch()
                {
                    Content = "oto",
                    Header = "Otomatik Ödeme Talimatı",
                    Url = "/Payment/AutomaticPayment"
                });
                SearchList.Add(new CMS.ViewModels.Home.QuickSearch()
                {
                    Content = "öde",
                    Header = "Otomatik Ödeme Talimatı",
                    Url = "/Payment/AutomaticPayment"
                });
                SearchList.Add(new CMS.ViewModels.Home.QuickSearch()
                {
                    Content = "öde",
                    Header = "Fatura & Ödemeler",
                    Url = "/Payment/BillsAndPayments"
                });
                SearchList.Add(new CMS.ViewModels.Home.QuickSearch()
                {
                    Content = "fat",
                    Header = "Otomatik Ödeme Talimatı",
                    Url = "/Payment/AutomaticPayment"
                });
                SearchList.Add(new CMS.ViewModels.Home.QuickSearch()
                {
                    Content = "fat",
                    Header = "Fatura & Ödemeler",
                    Url = "/Payment/BillsAndPayments"

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

                Session["quickSearch"] = content;
                return Content(content);
            }
            catch (Exception)
            {
                var content = "<div class='quick-search-result'>" +
                    "<div class='text-muted d-none'>No record found</div>" +
                    "<div class='font-size-sm text-primary font-weight-bolder text-uppercase mb-2'>Arama Sonuçları</div>" +
                    "<div class='mb-10'>" + "Sonuç Bulunamadı" +
                    "</div>" +
                    "</div>";
                return Content(content);
            }
        }
    }
}