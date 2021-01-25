using CMS.ViewModels.Auth;
using CustomerManagementSystem.GenericCustomerServiceRef;
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
        GenericCustomerServiceRef.GenericCustomerServiceClient client = new GenericCustomerServiceRef.GenericCustomerServiceClient();
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
                var result = client.CustomerAuthentication(new GenericCustomerServiceRef.CustomerServiceAuthenticationRequest()
                {
                    Culture = baseRequest.Culture,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    Hash = baseRequest.Hash,
                    AuthenticationParameters = new GenericCustomerServiceRef.AuthenticationRequest()
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
                var result = client.AuthenticationSMSConfirm(new GenericCustomerServiceRef.CustomerServiceAuthenticationSMSConfirmRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    AuthenticationSMSConfirmParameters = new GenericCustomerServiceRef.AuthenticationSMSConfirmRequest()
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
            return Json(new { valid = CMS.Localization.Errors.SMSPasswordWrong }, JsonRequestBehavior.AllowGet);
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
            var subscriptionId = Convert.ToInt64(ID);
            var customer = new ServiceUtilities().GetCustomerInfo(subscriptionId).GetCustomerInfoResponse;
            var isNotActive = customer == null || (customer.CustomerState == (int)CMS.Localization.Enums.CustomerState.PreRegisterd || (customer.CustomerState == (int)CMS.Localization.Enums.CustomerState.Registered || (customer.CustomerState == (int)CMS.Localization.Enums.CustomerState.Reserved)));
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, ValidDisplayName),
                new Claim(ClaimTypes.NameIdentifier, ID),
                new Claim(ClaimTypes.SerialNumber, SubscriberNo),
                new Claim("LastLogin",DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                new Claim("SubscriptionBag", string.Join(";", relatedCustomers)),
                new Claim("CurrentSubscriptionIsActive",isNotActive.ToString(),ClaimValueTypes.Boolean)
            }, "ApplicationCookie");
            if (!isNotActive)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "Customer"));
            }
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