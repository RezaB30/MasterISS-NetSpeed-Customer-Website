using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.ViewModels.Supports;
using NLog;
using PagedList;
using RezaB.Web.Authentication;

namespace CustomerManagementSystem.Controllers
{
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
                Date = s.Date,
                ApprovalDate = s.ApprovalDate,
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
            SupportMessagesViewModel supportMessages = new SupportMessagesViewModel()
            {
                SupportDisplayType = (SupportRequestDisplayTypes)supportDetails.SupportDetailMessagesResponse.SupportRequestDisplayType.SupportRequestDisplayTypeId,
                SupportRequestName = supportDetails.SupportDetailMessagesResponse.SupportRequestName,
                SupportRequestSummary = supportDetails.SupportDetailMessagesResponse.SupportRequestSubName,
                Department = supportDetails.SupportDetailMessagesResponse.SupportRequestName,
                SupportNo = supportDetails.SupportDetailMessagesResponse.SupportNo,
                State = supportDetails.SupportDetailMessagesResponse.State.StateName,
                SupportDate = supportDetails.SupportDetailMessagesResponse.SupportDate,
                ID = ID,
                StateId = supportDetails.SupportDetailMessagesResponse.State.StateId,
                SupportMessageList = supportDetails.SupportDetailMessagesResponse.SupportMessages.Select(s => new SupportMessageList()
                {
                    Files = null,
                    IsCustomer = s.IsCustomer,
                    SenderName = s.IsCustomer ? User.Identity.Name.Length > 20 ? User.Identity.Name.Substring(0, 20) + "..." : User.Identity.Name
                          : CMS.Localization.Common.Agent,
                    Message = s.Message,
                    MessageDate = s.MessageDate,

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
        public ActionResult NewRequests(NewRequestViewModel request) //yeni-talep.html
        {
            if (request.Description != null)
                request.Description = request.Description.Trim(new char[] { ' ', '\n', '\r' });
            ModelState.Clear();
            TryValidateModel(request);
            if (!ModelState.IsValid)
            {
                ViewBag.RequestTypes = new ServiceUtilities().GetSupportTypes(request.RequestTypeId);
                ViewBag.SubRequestTypes = new ServiceUtilities().GetSupportSubTypes(request.RequestTypeId, request.SubRequestTypeId);
                return View(request);
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
            var supportRegister = new ServiceUtilities().SupportRegister(request, User.GiveUserId());
            if (supportRegister.ResponseMessage.ErrorCode != 0)
            {
                return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), supportRegister.ResponseMessage.ErrorMessage, false);
            }
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
                Date = s.Date,
                ApprovalDate = s.ApprovalDate,
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
        public ActionResult NewSupportMessage(RequestSupportMessage requestMessage)
        {
            if (requestMessage.Message != null)
                requestMessage.Message = requestMessage.Message.Trim(new char[] { ' ', '\n', '\r' });
            if (requestMessage.IsSolved)
            {
                if (string.IsNullOrEmpty(requestMessage.Message))
                    requestMessage.Message = CMS.Localization.Common.ProblemSolved;
            }
            ModelState.Clear();
            TryValidateModel(requestMessage);
            if (!ModelState.IsValid)
            {
                return ReturnMessageUrl(Url.Action("SupportResults", "Support", new { requestMessage.ID }), ModelErrorMessages(ModelState), false);
            }
            var newMessageResponse = new ServiceUtilities().SendSupportMessage(requestMessage, User.GiveUserId());
            requestsLogger.Debug($"NewSupportMessage response -> ErrorCode : {newMessageResponse.ResponseMessage.ErrorCode} - Error Message : {newMessageResponse.ResponseMessage.ErrorMessage}");
            if (newMessageResponse.ResponseMessage.ErrorCode == 5) // has active request
            {
                return ReturnMessageUrl(Url.Action("SupportRequests", "Support"), newMessageResponse.ResponseMessage.ErrorMessage, false);
            }
            return ReturnMessageUrl(Url.Action("SupportResults", "Support", new { requestMessage.ID }), newMessageResponse.ResponseMessage.ErrorMessage, true);
        }        
        private string ModelErrorMessages(ModelStateDictionary ModelState)
        {
            return string.Join(Environment.NewLine, ModelState.Values.Select(m => string.Join(Environment.NewLine, m.Errors.Where(s => !string.IsNullOrEmpty(s.ErrorMessage)).Select(s => $"<div class='text-red'>{s.ErrorMessage}<div>"))));
        }
    }
}