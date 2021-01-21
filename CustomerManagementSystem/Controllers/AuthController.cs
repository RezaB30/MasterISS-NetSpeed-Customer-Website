using CMS.ViewModels.Auth;
using CustomerManagementSystem.GenericCustomerServiceReference;
using Microsoft.Owin;
using RezaB.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        GenericCustomerServiceReference.GenericCustomerServiceClient client = new GenericCustomerServiceReference.GenericCustomerServiceClient();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            ModelState.Remove("SMSPassword");
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            var result = new ServiceUtilities().AuthenticationWithPassword(login.CustomerCode, login.Password);
            if (result.ResponseMessage.ErrorCode != 0)
            {
                return Json(new { valid = result.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            // sign in
            SignInUser(result.AuthenticationWithPasswordResult.ValidDisplayName, result.AuthenticationWithPasswordResult.ID.ToString(), result.AuthenticationWithPasswordResult.SubscriberNo, result.AuthenticationWithPasswordResult.RelatedCustomers, Request.GetOwinContext());
            return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DirectLogin()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DirectLogin([Bind(Include = "CustomerCode")] LoginViewModel login)
        {
            ModelState.Remove("SMSPassword");
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                if (login.CustomerCode.StartsWith("0"))
                    login.CustomerCode = login.CustomerCode.TrimStart('0');

                var baseRequest = new GenericServiceSettings();
                var result = client.CustomerAuthentication(new GenericCustomerServiceReference.CustomerServiceAuthenticationRequest()
                {
                    Culture = baseRequest.Culture,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    Hash = baseRequest.Hash,
                    AuthenticationParameters = new GenericCustomerServiceReference.AuthenticationRequest()
                    {
                        CustomerCode = login.CustomerCode,
                    }
                });
                if (result.ResponseMessage.ErrorCode == 0)
                {
                    return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
                    //return View("SMSConfirm", login);
                }
                else
                {
                    var error = result.ResponseMessage.ErrorMessage;
                    return Json(new { valid = error, }, JsonRequestBehavior.AllowGet);
                    //ModelState.AddModelError("CustomerCode", result.ResponseMessage.ErrorMessage);
                }
            }
            var modelError = ModelState.Values.Select(m => m.Errors.Where(s => string.IsNullOrEmpty(s.ErrorMessage) == false).Select(s => s.ErrorMessage).FirstOrDefault()).FirstOrDefault();
            return Json(new { valid = modelError, }, JsonRequestBehavior.AllowGet);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CustomerLogin([Bind(Include = "CustomerCode,SMSPassword")] LoginViewModel login)
        {
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                if (login.CustomerCode.StartsWith("0"))
                    login.CustomerCode = login.CustomerCode.TrimStart('0');

                var baseRequest = new GenericServiceSettings();
                var result = client.AuthenticationSMSConfirm(new GenericCustomerServiceReference.CustomerServiceAuthenticationSMSConfirmRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    AuthenticationSMSConfirmParameters = new GenericCustomerServiceReference.AuthenticationSMSConfirmRequest()
                    {
                        CustomerCode = login.CustomerCode,
                        SMSPassword = login.SMSPassword
                    }
                });
                if (result.ResponseMessage.ErrorCode == 0)
                {
                    // sign in
                    SignInUser(result.AuthenticationSMSConfirmResponse.ValidDisplayName, result.AuthenticationSMSConfirmResponse.ID.ToString(), result.AuthenticationSMSConfirmResponse.SubscriberNo, result.AuthenticationSMSConfirmResponse.RelatedCustomers, Request.GetOwinContext());
                    return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
                    //return Redirect(GetRedirectUrl(Request.QueryString["ReturnUrl"]));
                }
                else
                {
                    return Json(new { valid = result.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
                    //ModelState.AddModelError("CustomerCode", result.ResponseMessage.ErrorMessage);
                }

            }
            return Json(new { valid = "Eksik yada Hatalı Şifre" }, JsonRequestBehavior.AllowGet);
            //return View(login);

            //if (password == "1")
            //{
            //    return Json(new { valid = "Eksik yada Hatalı Şifre" }, JsonRequestBehavior.AllowGet);
            //}
            //return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {
            SignoutUser(Request.GetOwinContext());
            return RedirectToAction("Login", "Auth");
        }
        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("Index", "Home");
            }
            return returnUrl;
        }
        internal static void SignInUser(string ValidDisplayName, string ID, string SubscriberNo, IEnumerable<string> relatedCustomers, IOwinContext context)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, ValidDisplayName),
                new Claim(ClaimTypes.NameIdentifier, ID),
                new Claim(ClaimTypes.SerialNumber, SubscriberNo),
                new Claim("LastLogin",DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                new Claim("SubscriptionBag", string.Join(";", relatedCustomers))
            }, "ApplicationCookie");
            context.Authentication.SignIn(identity);
        }

        internal static void SignInCurrentUserAgain(IOwinContext context)
        {
            GenericCustomerServiceClient serviceClient = new GenericCustomerServiceClient();
            var baseRequest = new GenericServiceSettings();
            var subId = context.Authentication.User.GiveUserId();
            var result = serviceClient.SubscriptionBasicInfo(new CustomerServiceBaseRequest()
            {
                Culture = baseRequest.Culture,
                Username = baseRequest.Username,
                Rand = baseRequest.Rand,
                Hash = baseRequest.Hash,
                SubscriptionParameters = new BaseSubscriptionRequest()
                {
                    SubscriptionId = subId
                }
            });
            if (result.ResponseMessage.ErrorCode == 0)
            {
                SignInUser(result.SubscriptionBasicInformationResponse.ValidDisplayName, result.SubscriptionBasicInformationResponse.ID.ToString(), result.SubscriptionBasicInformationResponse.SubscriberNo, result.SubscriptionBasicInformationResponse.RelatedCustomers, context);
            }
        }
        internal static void SignoutUser(IOwinContext context)
        {
            context.Authentication.SignOut();
        }
    }
}