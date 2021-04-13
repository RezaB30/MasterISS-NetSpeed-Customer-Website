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
        // GET: MobilePayment
        public ActionResult Index(long? id, long subscriptionId)
        {
            var response = Utilities.PaymentUtilities.Payment(id, subscriptionId);
            if (!response.IsSuccess)
            {
                return new ViewResult()
                {
                    ViewName = "~/Views/Shared/Error.cshtml",
                };
            }
            ViewBag.POSForm = response.HtmlForm; //VPOSFormResponse.VPOSFormResponse.HtmlForm;            
            TempData["POSForm"] = response.HtmlForm;
            var temp = new TempDataDictionary();
            temp.Add("POSForm", response.HtmlForm);
            return new ViewResult()
            {
                ViewName = "~/Views/Payment/3DHostPayment.cshtml",
                TempData = temp
            };
            //return View(viewName: "3DHostPayment");
        }
        public ActionResult VPOSSuccess(string id)
        {
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
            var response = Utilities.PaymentUtilities.VPOSFail(id);
            return RedirectToAction("Fail");
        }
    }
}