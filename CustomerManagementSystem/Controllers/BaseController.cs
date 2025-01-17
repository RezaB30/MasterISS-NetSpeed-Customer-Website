﻿using CustomerManagementSystem.Properties;
using NLog;
using RezaB.Web;
using RezaB.Web.Authentication;
using RezaB.Web.Extentions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CustomerManagementSystem.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        private static Logger logger = LogManager.GetLogger("main");
        private static Logger requestLogger = LogManager.GetLogger("requestLogger");
        private static Logger exceptionLogger = LogManager.GetLogger("exceptions");
        private static Logger failedRequestLogger = LogManager.GetLogger("failedRequestLogger");

        protected override void OnException(ExceptionContext filterContext)
        {
            exceptionLogger.Error(filterContext.Exception);
            var routeData = string.Join("|", filterContext.RouteData.Values.Select(r => $"{r.Key} : {r.Value}"));
            exceptionLogger.Error($"RouteData > {routeData}");
            //if (filterContext.ExceptionHandled)
            //{
            //    return;
            //}
            //filterContext.Result = new ViewResult
            //{
            //    ViewName = "~/Views/Shared/Error.cshtml"
            //};
            //filterContext.ExceptionHandled = true;
        }
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            // log request
            if (Request.IsAuthenticated)
            {
                var cookies = new List<string>();
                foreach (var key in Request.Cookies.AllKeys)
                {
                    cookies.Add(key + ": " + Request.Cookies[key].Value);
                }
                var eventLog = new LogEventInfo(LogLevel.Trace, "requestLogger", "Request:");
                eventLog.Properties["subscriberNo"] = User.GiveUserId();
                eventLog.Properties["url"] = Request.Url.AbsoluteUri;
                eventLog.Properties["userAgent"] = Request.UserAgent;
                eventLog.Properties["httpMethod"] = Request.HttpMethod;
                eventLog.Properties["userHostAddress"] = Request.UserHostAddress;
                eventLog.Properties["userCookies"] = string.Join(Environment.NewLine, cookies);
                var log = string.Join(Environment.NewLine, eventLog.Properties.Select(ev => $"{ev.Key} : {ev.Value}").ToArray());
                requestLogger.Info(log);
            }
            else
            {
                var cookies = new List<string>();
                foreach (var key in Request.Cookies.AllKeys)
                {
                    cookies.Add(key + ": " + Request.Cookies[key].Value);
                }
                var eventLog = new LogEventInfo(LogLevel.Trace, "requestLogger", "Request:");
                eventLog.Properties["subscriberNo"] = "anonymus";
                eventLog.Properties["url"] = Request.Url.AbsoluteUri;
                eventLog.Properties["userAgent"] = Request.UserAgent;
                eventLog.Properties["httpMethod"] = Request.HttpMethod;
                eventLog.Properties["userHostAddress"] = Request.UserHostAddress;
                eventLog.Properties["userCookies"] = string.Join(Environment.NewLine, cookies);
                var log = string.Join(Environment.NewLine, eventLog.Properties.Select(ev => $"{ev.Key} : {ev.Value}").ToArray());
                failedRequestLogger.Info(log);
            }
        }
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            //Localization in Base controller:
            string lang = CookieTools.getCulture(Request.Cookies);
            var routeData = RouteData.Values;
            var routeCulture = routeData.Where(r => r.Key == "lang").FirstOrDefault();
            if (string.IsNullOrEmpty((string)routeCulture.Value) || routeCulture.Value.ToString() != lang)
            {
                routeData.Remove("lang");
                routeData.Add("lang", lang);

                Thread.CurrentThread.CurrentUICulture =
                Thread.CurrentThread.CurrentCulture =
                CultureInfo.GetCultureInfo(lang);

                Response.RedirectToRoute(routeData);
            }
            else
            {
                lang = (string)RouteData.Values["lang"];

                Thread.CurrentThread.CurrentUICulture =
                    Thread.CurrentThread.CurrentCulture =
                    CultureInfo.GetCultureInfo(lang);
            }

            ViewBag.Version = Settings.Default.Version;
            ViewBag.Copyright = Settings.Default.Copyright;
            ViewBag.WebSite = Settings.Default.WebSite;
            ViewBag.CompanyTitle = Settings.Default.CompanyTitle;
            return base.BeginExecuteCore(callback, state);
        }
        [AllowAnonymous]
        [HttpGet, ActionName("Language")]
        public virtual ActionResult Language(string culture, string sender)
        {
            CookieTools.SetCultureInfo(Response.Cookies, culture);

            Dictionary<string, object> responseParams = new Dictionary<string, object>();
            Request.QueryString.CopyTo(responseParams);
            responseParams.Add("lang", culture);
            return RedirectToAction("Index", "Home");
            //return RedirectToAction(sender, new RouteValueDictionary(responseParams));
        }
        public virtual ActionResult ReturnMessageUrl(string url, string message = null, bool? IsSuccess = null)
        {
            if (IsSuccess == false && !string.IsNullOrEmpty(message))
            {
                TempData["generic-error"] = message;
            }
            if (IsSuccess == true && !string.IsNullOrEmpty(message))
            {
                TempData["generic-success"] = message;
            }
            return Redirect(url);
        }
        public virtual ActionResult GetUnpaidBills()
        {
            var bills = new ServiceUtilities().GetCustomerBillList(User.GiveUserId());
            if (bills.ResponseMessage.ErrorCode != 0 || bills.GetCustomerBillsResponse.CustomerBills == null || bills.GetCustomerBillsResponse.CustomerBills.Count() == 0)
            {
                return PartialView("~/Views/Shared/LayoutParts/_BillInfo.cshtml", new CMS.ViewModels.Layout.BillsLayoutViewModel());
            }
            var unpaidBills = bills.GetCustomerBillsResponse.CustomerBills.Where(bill => bill.Status == (short)CMS.Localization.Enums.BillState.Unpaid).Select(bill => new CMS.ViewModels.Layout.BillsLayoutViewModel.BillList()
            {
                Amount = bill.Total.ToString("###,##0.00"),
                IssueDate = Utilities.InternalUtilities.DateTimeConverter.ParseDateTime(bill.BillDate)
            });
            var TotalAmount = bills.GetCustomerBillsResponse.CustomerBills.Where(bill => bill.Status == (short)CMS.Localization.Enums.BillState.Unpaid).Sum(bill => bill.Total);
            var results = new CMS.ViewModels.Layout.BillsLayoutViewModel()
            {
                Bills = unpaidBills ?? Enumerable.Empty<CMS.ViewModels.Layout.BillsLayoutViewModel.BillList>(),
                BillCount = unpaidBills.Count(),
                TotalAmount = TotalAmount.ToString("###,##0.00")
            };
            return PartialView("~/Views/Shared/LayoutParts/_BillInfo.cshtml", results);
        }
        public virtual ActionResult Error(string message, string details)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(new { Code = 1, Message = message, Details = details }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.Message = message;
            ViewBag.Details = details;
            return View("ErrorDialogBox");
        }
        protected void SetupPages<T>(int? page, ref IQueryable<T> viewResults, int? rowCount = null)
        {
            rowCount = rowCount ?? Settings.Default.TableRows;
            var totalCount = viewResults.Count();
            var pagesCount = Math.Ceiling((float)totalCount / (float)rowCount);
            ViewBag.PageCount = pagesCount;
            ViewBag.PageTotalCount = totalCount;

            if (!page.HasValue)
            {
                page = 0;
            }

            viewResults = viewResults.PageData(page.Value, rowCount.Value);
        }
        protected string ViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public virtual ActionResult Notifications()
        {
            try
            {
                var statusResponse = new ServiceUtilities().SupportStatus(User.GiveUserId());
                long[] supportIds = { };
                if (statusResponse.ResponseMessage.ErrorCode == 0 && statusResponse.SupportStatusResponse != null && statusResponse.SupportStatusResponse.StageId != null)
                {
                    supportIds = new long[] { statusResponse.SupportStatusResponse.StageId.GetValueOrDefault(0) };
                }
                var notifications = new List<CMS.ViewModels.Supports.NotificationViewModel>();
                foreach (var item in supportIds)
                {
                    if (Request.Cookies["Notification_" + item.ToString()] == null)
                    {
                        var url = Url.Action("SupportResults", "Support", new { ID = item });
                        notifications.Add(new CMS.ViewModels.Supports.NotificationViewModel()
                        {
                            Url = Url.Action("RedirectNotification", new { url = url, uniqueId = item }),
                            Content = CMS.Localization.Common.NewSupportNotification,
                            Type = CMS.ViewModels.Supports.NotificationType.Info
                        });
                    }
                }
                return PartialView("_Notifications", notifications);
            }
            catch (Exception)
            {
                return PartialView("_Notifications", Enumerable.Empty<CMS.ViewModels.Supports.NotificationViewModel>());
            }
        }
        public ActionResult RedirectNotification(string url, string uniqueId)
        {
            var cookie = new HttpCookie($"Notification_{uniqueId}")
            {
                Expires = DateTime.Now + new TimeSpan(0, 2, 0)
            };
            var response = HttpContext.Response;
            response.Cookies.Add(cookie);
            return Redirect(url);
        }
    }
}