using NLog;
using RezaB.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        private static Logger logger = LogManager.GetLogger("main");
        private static Logger requestLogger = LogManager.GetLogger("requestLogger");

        protected override void OnException(ExceptionContext filterContext)
        {
            logger.Error(filterContext.Exception);
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
                requestLogger.Log(eventLog);
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
                requestLogger.Log(eventLog);
            }
        }
    }
}