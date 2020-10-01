using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Controllers
{
    public class InitializeController : Controller
    {
        // GET: Initialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"> resource text value</param>
        /// <param name="type"> Message </param>
        /// <returns></returns>
        public ActionResult Init(string text) // resource display text for javascript
        {
            var displayText = CMS.Localization.Message.ResourceManager.GetString(text, Thread.CurrentThread.CurrentUICulture);
            return Json(new { displayText }, JsonRequestBehavior.AllowGet);
        }
    }
}