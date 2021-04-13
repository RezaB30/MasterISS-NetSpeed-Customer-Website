using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.ViewModels.Supports;
using NLog;
using PagedList;
using RezaB.Web.Authentication;

namespace CustomerManagementSystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class SupportController : BaseController
    {
        Logger requestsLogger = LogManager.GetLogger("requests");
        // GET: Support
        public ActionResult SupportRequests(int? page)   // destek.html
        {
            var supportRequests = new ServiceUtilities().GetSupportRequests(User.GiveUserId());
            if (supportRequests.ResponseMessage.ErrorCode != 0)
            {
                return View(Enumerable.Empty<SupportRequestsViewModel>());
            }
            Dictionary<long, SupportRequestsAdditional> keyValuePairs = new Dictionary<long, SupportRequestsAdditional>();
            foreach (var item in supportRequests.GetCustomerSupportListResponse)
            {
                var supportDetailsResponse = new ServiceUtilities().GetSupportDetails(User.GiveUserId(), item.ID);
                if (supportDetailsResponse.ResponseMessage.ErrorCode == 0)
                {
                    var supportDetails = new
                    {
                        lastMessage = supportDetailsResponse.SupportDetailMessagesResponse.SupportMessages.OrderByDescending(s => s.MessageDate).FirstOrDefault().Message,
                        IsCustomer = supportDetailsResponse.SupportDetailMessagesResponse.SupportMessages.OrderByDescending(s => s.MessageDate).FirstOrDefault().IsCustomer,
                        supportDisplayType = supportDetailsResponse.SupportDetailMessagesResponse.SupportRequestDisplayType.SupportRequestDisplayTypeId
                    };
                    keyValuePairs.Add(item.ID, new SupportRequestsAdditional()
                    {
                        IsCustomer = supportDetails.IsCustomer,
                        LastMessage = supportDetails.lastMessage,
                        SupportDisplayType = supportDetails.supportDisplayType

                    });
                }
            }
            var supportRequestList = supportRequests.GetCustomerSupportListResponse != null ? supportRequests.GetCustomerSupportListResponse.OrderByDescending(s => s.Date).Select(s => new SupportRequestsViewModel()
            {
                SupportDisplayType = (SupportRequestDisplayTypes)keyValuePairs.Where(dic => dic.Key == s.ID).FirstOrDefault().Value.SupportDisplayType,
                SupportId = s.ID,
                Date = Utilities.InternalUtilities.DateTimeConverter.ParseDateTime(s.Date).Value,
                ApprovalDate = Utilities.InternalUtilities.DateTimeConverter.ParseDateTime(s.ApprovalDate),
                SupportNo = s.SupportNo,
                State = s.StateText,
                StateId = s.State,
                Department = s.SupportRequestType,
                SupportRequestType = s.SupportRequestType,
                SupportRequestSubType = s.SupportRequestSubType,
                Note = keyValuePairs.Where(dic => dic.Key == s.ID).FirstOrDefault().Value.LastMessage,
                ChangeType = keyValuePairs.Where(dic => dic.Key == s.ID).FirstOrDefault().Value.IsCustomer ? ChangeType.Customer : ChangeType.Agent

            }).AsQueryable() : Enumerable.Empty<SupportRequestsViewModel>().AsQueryable();

            SetupPages(page, ref supportRequestList, 10);

            return View(supportRequestList.ToList());
        }
        public ActionResult SupportResults(long ID)
        {
            var supportDetails = new ServiceUtilities().GetSupportDetails(User.GiveUserId(), ID);
            if (supportDetails.ResponseMessage.ErrorCode != 0)
            {
                return RedirectToAction("SupportRequests", "Support");
            }
            var attachments = new ServiceUtilities().GetSupportAttachmentList(ID);
            var genericAppSetting = new ServiceUtilities().CustomerWebsiteGenericSettings();
            var fileMaxCount = genericAppSetting.GenericAppSettings == null ? 20 : genericAppSetting.GenericAppSettings.FileMaxCount;
            var fileMaxSize = genericAppSetting.GenericAppSettings == null ? 5242880 : genericAppSetting.GenericAppSettings.FileMaxSize;
            ViewBag.FileMaxSize = fileMaxSize;
            ViewBag.CanSendFile = false;
            if (attachments.GetSupportAttachmentList == null || attachments.GetSupportAttachmentList.Count() == 0 || attachments.GetSupportAttachmentList.Count() < fileMaxCount)
            {
                ViewBag.CanSendFile = true;
            }
            SupportMessagesViewModel supportMessages = new SupportMessagesViewModel()
            {
                SupportDisplayType = (SupportRequestDisplayTypes)supportDetails.SupportDetailMessagesResponse.SupportRequestDisplayType.SupportRequestDisplayTypeId,
                SupportRequestName = supportDetails.SupportDetailMessagesResponse.SupportRequestName,
                SupportRequestSummary = supportDetails.SupportDetailMessagesResponse.SupportRequestSubName,
                Department = supportDetails.SupportDetailMessagesResponse.SupportRequestName,
                SupportNo = supportDetails.SupportDetailMessagesResponse.SupportNo,
                State = supportDetails.SupportDetailMessagesResponse.State.StateName,
                SupportDate = Utilities.InternalUtilities.DateTimeConverter.ParseDateTime(supportDetails.SupportDetailMessagesResponse.SupportDate).Value,
                ID = ID,
                StateId = supportDetails.SupportDetailMessagesResponse.State.StateId,
                SupportMessageList = supportDetails.SupportDetailMessagesResponse.SupportMessages.Select(s => new SupportMessageList()
                {
                    Files = null,
                    IsCustomer = s.IsCustomer,
                    SenderName = s.IsCustomer ? User.Identity.Name.Length > 20 ? User.Identity.Name.Substring(0, 20) + "..." : User.Identity.Name
                          : CMS.Localization.Common.Agent,
                    Message = s.Message,
                    MessageDate = Utilities.InternalUtilities.DateTimeConverter.ParseDateTime(s.MessageDate).Value,
                    StageId = s.StageId
                })
            };
            return View(supportMessages);
        }
        public ActionResult NewRequests() //yeni-talep.html
        {
            var hasActiveRequest = new ServiceUtilities().HasOpenRequest(User.GiveUserId());
            if (hasActiveRequest.ResponseMessage.ErrorCode != 0)
            {
                return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), hasActiveRequest.ResponseMessage.ErrorMessage, false);
            }
            if (hasActiveRequest.HasActiveRequest == true)
            {
                return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), CMS.Localization.Errors.HasActiveRequest, false);
            }
            ViewBag.RequestTypes = new ServiceUtilities().GetSupportTypes();
            ViewBag.SubRequestTypes = new SelectList(Enumerable.Empty<object>());
            var genericAppSetting = new ServiceUtilities().CustomerWebsiteGenericSettings();
            var fileMaxCount = genericAppSetting.GenericAppSettings == null ? 20 : genericAppSetting.GenericAppSettings.FileMaxCount;
            var fileMaxSize = genericAppSetting.GenericAppSettings == null ? 5242880 : genericAppSetting.GenericAppSettings.FileMaxSize;
            ViewBag.FileMaxSize = fileMaxSize;
            return View();
        }
        [HttpPost]
        public ActionResult GetSubRequestTypes(int RequestTypeId)
        {
            var list = new ServiceUtilities().GetSupportSubTypes(RequestTypeId).Select(sst => new { Text = sst.Text, Value = sst.Value }).ToList();
            list.Insert(0, new { Text = CMS.Localization.Common.Choose, Value = "" });
            return Json(list.ToArray());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewRequests(NewRequestViewModel request, HttpPostedFileBase[] attachments) //yeni-talep.html
        {
            if (request.Description != null)
                request.Description = request.Description.Trim(new char[] { ' ', '\n', '\r' });
            attachments = attachments != null ? attachments.Where(att => att != null).FirstOrDefault() == null ? null : attachments : attachments;
            if (attachments != null)
            {
                if (string.IsNullOrEmpty(request.Description))
                    request.Description = CMS.Localization.Common.FileAdded;
            }
            ModelState.Clear();
            TryValidateModel(request);
            var genericAppSetting = new ServiceUtilities().CustomerWebsiteGenericSettings();
            var fileMaxSize = genericAppSetting.GenericAppSettings == null ? 5242880 : genericAppSetting.GenericAppSettings.FileMaxSize;
            var fileMaxCount = genericAppSetting.GenericAppSettings == null ? 20 : genericAppSetting.GenericAppSettings.FileMaxCount;
            if (!ModelState.IsValid)
            {
                ViewBag.RequestTypes = new ServiceUtilities().GetSupportTypes(request.RequestTypeId);
                ViewBag.SubRequestTypes = new ServiceUtilities().GetSupportSubTypes(request.RequestTypeId, request.SubRequestTypeId);
                ViewBag.FileMaxSize = fileMaxSize;
                return View(request);
            }
            if (attachments != null && attachments.Where(att => att.ContentLength > fileMaxSize).FirstOrDefault() != null)
            {
                return ReturnMessageUrl(Url.Action("NewRequests", "Support"), string.Format(CMS.Localization.Errors.FileSizeError, (fileMaxSize / 1000000)), false);
            }
            var hasActiveRequest = new ServiceUtilities().HasOpenRequest(User.GiveUserId());
            if (hasActiveRequest.ResponseMessage.ErrorCode != 0)
            {
                return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), hasActiveRequest.ResponseMessage.ErrorMessage, false);
            }
            if (hasActiveRequest.HasActiveRequest == true)
            {
                return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), CMS.Localization.Errors.HasActiveRequest, false);
            }
            List<MasterISS.CustomerService.NetspeedCustomerServiceReference.Attachment> attachmentList = new List<MasterISS.CustomerService.NetspeedCustomerServiceReference.Attachment>();
            if (attachments != null && attachments.Count() != 0 && attachments.Count() <= fileMaxCount)
            {
                foreach (var item in attachments)
                {
                    using (var binaryReader = new BinaryReader(item.InputStream))
                    {
                        var attachmentByte = binaryReader.ReadBytes(item.ContentLength);
                        FileInfo fileInfo = new FileInfo(item.FileName);
                        var fileExtension = fileInfo.Extension.Replace(".", "");
                        attachmentList.Add(new MasterISS.CustomerService.NetspeedCustomerServiceReference.Attachment()
                        {
                            FileContent = attachmentByte,
                            FileExtention = fileExtension,
                            FileName = item.FileName
                        });
                    }
                }
            }
            var supportRegister = new ServiceUtilities().SupportRegister(request, attachmentList, User.GiveUserId());
            if (supportRegister.ResponseMessage.ErrorCode != 0)
            {
                return ReturnMessageUrl(Url.Action("NewRequests", "Support"), supportRegister.ResponseMessage.ErrorMessage, false);
            }
            //add file
            //if (attachments != null && attachments.Count() != 0 && attachments.Count() <= fileMaxCount)
            //{
            //    var supportRequests = new ServiceUtilities().GetSupportRequests(User.GiveUserId());
            //    if (supportRequests.ResponseMessage.ErrorCode != 0)
            //    {
            //        requestsLogger.Error(supportRequests.ResponseMessage.ErrorMessage);
            //        return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), CMS.Localization.Errors.InternalErrorDescription, false);
            //    }
            //    var supportDetails = new ServiceUtilities().GetSupportDetails(User.GiveUserId(), supportRegister.SupportRegisterResponse.SupportId);
            //    if (supportDetails.ResponseMessage.ErrorCode != 0)
            //    {
            //        requestsLogger.Error(supportDetails.ResponseMessage.ErrorMessage);
            //        return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), CMS.Localization.Errors.InternalErrorDescription, false);
            //    }
            //    if (supportDetails.SupportDetailMessagesResponse == null || supportDetails.SupportDetailMessagesResponse.SupportMessages == null)
            //    {
            //        requestsLogger.Error($"Support Message List Not Found. Subscription Id : {User.GiveUserId()} - Support Id : {supportRegister.SupportRegisterResponse.SupportId}");
            //        return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), CMS.Localization.Errors.InternalErrorDescription, false);
            //    }
            //    var stageId = supportDetails.SupportDetailMessagesResponse.SupportMessages.FirstOrDefault().StageId;
            //    // save attachment
            //    foreach (var item in attachments)
            //    {
            //        using (var binaryReader = new BinaryReader(item.InputStream))
            //        {
            //            var attachmentByte = binaryReader.ReadBytes(item.ContentLength);
            //            FileInfo fileInfo = new FileInfo(item.FileName);
            //            var fileExtension = fileInfo.Extension.Replace(".", "");
            //            var saveAttachment = new ServiceUtilities().SaveSupportAttachment(stageId, item.FileName, attachmentByte, fileExtension, supportRegister.SupportRegisterResponse.SupportId);
            //            requestsLogger.Info($"Save Attachment Service Response | Error Code : {saveAttachment.ResponseMessage.ErrorCode} - Error Message : {saveAttachment.ResponseMessage.ErrorMessage}");
            //        }
            //    }
            //}
            return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), supportRegister.ResponseMessage.ErrorMessage, true);
        }
        public ActionResult SupportRequestTable()
        {
            var supportRequests = new ServiceUtilities().GetSupportRequests(User.GiveUserId());
            if (supportRequests.ResponseMessage.ErrorCode != 0)
            {
                return PartialView("~/Views/Shared/DisplayPartials/Index/RequestsTable.cshtml", Enumerable.Empty<SupportRequestsViewModel>());
            }
            Dictionary<long, SupportRequestsAdditional> keyValuePairs = new Dictionary<long, SupportRequestsAdditional>();
            foreach (var item in supportRequests.GetCustomerSupportListResponse)
            {
                var supportDetailsResponse = new ServiceUtilities().GetSupportDetails(User.GiveUserId(), item.ID);
                if (supportDetailsResponse.ResponseMessage.ErrorCode == 0)
                {
                    var supportDetails = new
                    {
                        lastMessage = supportDetailsResponse.SupportDetailMessagesResponse.SupportMessages.OrderByDescending(s => s.MessageDate).FirstOrDefault().Message,
                        IsCustomer = supportDetailsResponse.SupportDetailMessagesResponse.SupportMessages.OrderByDescending(s => s.MessageDate).FirstOrDefault().IsCustomer
                    };
                    keyValuePairs.Add(item.ID, new SupportRequestsAdditional()
                    {
                        IsCustomer = supportDetails.IsCustomer,
                        LastMessage = supportDetails.lastMessage
                    });
                }
            }
            var supportRequestList = supportRequests.GetCustomerSupportListResponse != null ? supportRequests.GetCustomerSupportListResponse.OrderByDescending(s => s.Date).Take(Properties.Settings.Default.SupportTableRow).Select(s => new SupportRequestsViewModel()
            {
                SupportId = s.ID,
                Date = Utilities.InternalUtilities.DateTimeConverter.ParseDateTime(s.Date).Value,
                ApprovalDate = Utilities.InternalUtilities.DateTimeConverter.ParseDateTime(s.ApprovalDate),
                SupportNo = s.SupportNo,
                State = s.StateText,
                StateId = s.State,
                Department = s.SupportRequestType,
                SupportRequestType = s.SupportRequestType,
                SupportRequestSubType = s.SupportRequestSubType,
                Note = keyValuePairs.Where(dic => dic.Key == s.ID).FirstOrDefault().Value.LastMessage,
                ChangeType = keyValuePairs.Where(dic => dic.Key == s.ID).FirstOrDefault().Value.IsCustomer ? ChangeType.Customer : ChangeType.Agent

            }).ToArray() : Enumerable.Empty<SupportRequestsViewModel>().ToArray();
            return PartialView("~/Views/Shared/DisplayPartials/Index/RequestsTable.cshtml", supportRequestList.OrderByDescending(m => m.Date).ToList());
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult NewSupportMessage(RequestSupportMessage requestMessage, HttpPostedFileBase[] attachments)
        {
            if (requestMessage.Message != null)
                requestMessage.Message = requestMessage.Message.Trim(new char[] { ' ', '\n', '\r' });
            if (requestMessage.IsSolved)
            {
                if (string.IsNullOrEmpty(requestMessage.Message))
                    requestMessage.Message = CMS.Localization.Common.ProblemSolved;
            }
            attachments = attachments != null ? attachments.Where(att => att != null).FirstOrDefault() == null ? null : attachments : attachments;
            if (attachments != null)
            {
                if (string.IsNullOrEmpty(requestMessage.Message))
                    requestMessage.Message = CMS.Localization.Common.FileAdded;
            }
            ModelState.Clear();
            TryValidateModel(requestMessage);
            if (!ModelState.IsValid)
            {
                return ReturnMessageUrl(Url.Action("SupportResults", "Support", new { requestMessage.ID }), ModelErrorMessages(ModelState), false);
            }

            var genericAppSetting = new ServiceUtilities().CustomerWebsiteGenericSettings();
            var fileMaxSize = genericAppSetting.GenericAppSettings == null ? 5242880 : genericAppSetting.GenericAppSettings.FileMaxSize;
            if (attachments != null && attachments.Where(att => att.ContentLength > fileMaxSize).FirstOrDefault() != null)
            {
                return ReturnMessageUrl(Url.Action("SupportResults", "Support", new { requestMessage.ID }), string.Format(CMS.Localization.Errors.FileSizeError, (fileMaxSize / 1000000)), false);
            }
            List<MasterISS.CustomerService.NetspeedCustomerServiceReference.Attachment> attachmentList = new List<MasterISS.CustomerService.NetspeedCustomerServiceReference.Attachment>();
            if (attachments != null && attachments.Count() != 0)
            {
                var fileMaxCount = genericAppSetting.GenericAppSettings == null ? 20 : genericAppSetting.GenericAppSettings.FileMaxCount;
                var attachmentsList = new ServiceUtilities().GetSupportAttachmentList(requestMessage.ID);
                if (attachmentsList.GetSupportAttachmentList == null || attachmentsList.GetSupportAttachmentList.Count() == 0
                    || (attachments.Count() + attachmentsList.GetSupportAttachmentList.Count()) <= fileMaxCount)
                {
                    foreach (var item in attachments)
                    {
                        using (var binaryReader = new BinaryReader(item.InputStream))
                        {
                            var attachmentByte = binaryReader.ReadBytes(item.ContentLength);
                            FileInfo fileInfo = new FileInfo(item.FileName);
                            var fileExtension = fileInfo.Extension.Replace(".", "");
                            attachmentList.Add(new MasterISS.CustomerService.NetspeedCustomerServiceReference.Attachment()
                            {
                                FileContent = attachmentByte,
                                FileExtention = fileExtension,
                                FileName = item.FileName
                            });
                            //var saveAttachment = new ServiceUtilities().SaveSupportAttachment(newMessageResponse.SendSupportMessageResponse.Value, item.FileName, attachmentByte, fileExtension, requestMessage.ID);
                            //requestsLogger.Info($"Save Attachment Service Response | Error Code : {saveAttachment.ResponseMessage.ErrorCode} - Error Message : {saveAttachment.ResponseMessage.ErrorMessage}");
                        }
                    }
                }
                else
                {
                    return ReturnMessageUrl(Url.Action("SupportResults", "Support", new { requestMessage.ID }), CMS.Localization.Errors.FileUploadError, false);
                }
            }
            var newMessageResponse = new ServiceUtilities().SendSupportMessage(requestMessage, attachmentList, User.GiveUserId());
            requestsLogger.Debug($"NewSupportMessage response -> ErrorCode : {newMessageResponse.ResponseMessage.ErrorCode} - Error Message : {newMessageResponse.ResponseMessage.ErrorMessage}");
            if (newMessageResponse.ResponseMessage.ErrorCode == 5) // has active request
            {
                return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), newMessageResponse.ResponseMessage.ErrorMessage, false);
            }
            //if (attachments != null && attachments.Count() != 0)
            //{
            //    var fileMaxCount = genericAppSetting.GenericAppSettings == null ? 20 : genericAppSetting.GenericAppSettings.FileMaxCount;
            //    var attachmentsList = new ServiceUtilities().GetSupportAttachmentList(requestMessage.ID);
            //    if (attachmentsList.GetSupportAttachmentList == null || attachmentsList.GetSupportAttachmentList.Count() == 0
            //        || (attachments.Count() + attachmentsList.GetSupportAttachmentList.Count()) <= fileMaxCount)
            //    {
            //        if (newMessageResponse.ResponseMessage.ErrorCode == 0)
            //        {
            //            // save attachment
            //            foreach (var item in attachments)
            //            {
            //                using (var binaryReader = new BinaryReader(item.InputStream))
            //                {
            //                    var attachmentByte = binaryReader.ReadBytes(item.ContentLength);
            //                    FileInfo fileInfo = new FileInfo(item.FileName);
            //                    var fileExtension = fileInfo.Extension.Replace(".", "");
            //                    var saveAttachment = new ServiceUtilities().SaveSupportAttachment(newMessageResponse.SendSupportMessageResponse.Value, item.FileName, attachmentByte, fileExtension, requestMessage.ID);
            //                    requestsLogger.Info($"Save Attachment Service Response | Error Code : {saveAttachment.ResponseMessage.ErrorCode} - Error Message : {saveAttachment.ResponseMessage.ErrorMessage}");
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        return ReturnMessageUrl(Url.Action("SupportResults", "Support", new { requestMessage.ID }), CMS.Localization.Errors.FileUploadError, false);
            //    }
            //}

            return ReturnMessageUrl(Url.Action("SupportResults", "Support", new { requestMessage.ID }), newMessageResponse.ResponseMessage.ErrorMessage, true);
        }
        [HttpPost]
        public ActionResult GetSupportAttachments(long supportId)
        {
            var attachments = new ServiceUtilities().GetSupportAttachmentList(supportId);
            if (attachments.GetSupportAttachmentList == null)
            {
                return Json(Enumerable.Empty<object>(), JsonRequestBehavior.AllowGet);
            }
            return Json(attachments.GetSupportAttachmentList.Select(a => new { FileName = a.FileName, ServerName = a.ServerSideName, StageId = a.StageId }).ToArray(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadSupportAttachment(long? supportId, string fileName)
        {
            var attachment = new ServiceUtilities().GetSupportAttachment(supportId, fileName);
            if (attachment.ResponseMessage.ErrorCode != 0)
            {
                return ReturnMessageUrl(Url.Action("SupportResults", "Support", new { ID = supportId }), attachment.ResponseMessage.ErrorMessage, true);
            }
            if (attachment.GetSupportAttachment == null)
            {
                return ReturnMessageUrl(Url.Action("SupportResults", "Support", new { ID = supportId }), CMS.Localization.Errors.InternalErrorDescription, true);
            }
            return File(attachment.GetSupportAttachment.FileContent, attachment.GetSupportAttachment.MIMEType, $"{attachment.GetSupportAttachment.FileName}.{attachment.GetSupportAttachment.FileExtention}");
        }
        private string ModelErrorMessages(ModelStateDictionary ModelState)
        {
            return string.Join(Environment.NewLine, ModelState.Values.Select(m => string.Join(Environment.NewLine, m.Errors.Where(s => !string.IsNullOrEmpty(s.ErrorMessage)).Select(s => $"<div class='text-red'>{s.ErrorMessage}<div>"))));
        }
    }
}