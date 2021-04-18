using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Controllers
{
    [AllowAnonymous]
    public class MobilePaymentController : BaseController
    {
        Logger paymentLogger = LogManager.GetLogger("payments");
        Logger unpaidLogger = LogManager.GetLogger("unpaid");
        // GET: MobilePayment
        public ActionResult Index(long? id, long subscriptionId)
        {
            var response = Utilities.PaymentUtilities.Payment(id, subscriptionId);
            if (!response.IsSuccess)
            {
                return Content(response.ResponseMessage);
            }
            return Content(response.HtmlForm);
        }
        public ActionResult VPOSSuccess(string id)
        {
            paymentLogger.Debug("Mobile Payment");
            var response = Utilities.PaymentUtilities.VPOSSuccess(id);
            return RedirectToAction("Successful");
        }
        public ActionResult Successful()
        {
            return Content(Session["POSErrorMessage"] as string);
        }
        public ActionResult Fail()
        {
            return Content(Session["POSErrorMessage"] as string);
        }
        public ActionResult VPOSFail(string id)
        {
            unpaidLogger.Debug("Mobile Payment");
            var response = Utilities.PaymentUtilities.VPOSFail(id);
            return RedirectToAction("Fail");
        }
    }
}