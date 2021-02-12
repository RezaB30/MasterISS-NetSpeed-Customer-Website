using CMS.ViewModels.Auth;
using MasterISS.CustomerService.NetspeedCustomerServiceReference;
//using CustomerManagementSystem.NetspeedCustomerServiceReference;
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
        NetspeedCustomerServiceClient client = new NetspeedCustomerServiceClient();
        // GET: Account
        public ActionResult Login()
        {
            Session.Remove("HasCustomCaptcha");
            var googleRecaptcha = new ServiceUtilities().CustomerWebsiteGenericSettings();
            ViewBag.clientCaptchaKey = googleRecaptcha.GenericAppSettings == null ? "" : googleRecaptcha.GenericAppSettings.RecaptchaClientKey;
            if (googleRecaptcha.GenericAppSettings != null && !googleRecaptcha.GenericAppSettings.UseGoogleRecaptcha)
            {
                Session["HasCustomCaptcha"] = true;
            }
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            var invalidCaptcha = Session["HasCustomCaptcha"];
            if (invalidCaptcha != null)
            {
                var customCaptcha = Request.Form["customCaptcha"];
                var loginCaptcha = Session["LoginCaptcha"] as string;
                if (customCaptcha != loginCaptcha)
                {
                    return Json(new { invalidCaptcha = true, valid = CMS.Localization.Errors.InvalidCaptcha }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var googleRecaptcha = new ServiceUtilities().CustomerWebsiteGenericSettings();
                var recaptchaServerkey = googleRecaptcha.GenericAppSettings == null ? "" : googleRecaptcha.GenericAppSettings.RecaptchaServerKey;
                var captchaResponseKey = Request.Form["g-Recaptcha-Response"];
                var captcha = RezaB.Web.Captcha.GoogleRecaptchaValidator.Check(recaptchaServerkey, captchaResponseKey);
                // captcha control end
                if (captcha == RezaB.Web.Captcha.GoogleRecaptchaResultType.Fail)
                {
                    return Json(new { valid = CMS.Localization.Errors.InvalidCaptcha }, JsonRequestBehavior.AllowGet);
                }
                if (captcha == RezaB.Web.Captcha.GoogleRecaptchaResultType.NotWorking)
                {
                    return Json(new { invalidCaptcha = true, valid = CMS.Localization.Errors.InvalidCaptcha }, JsonRequestBehavior.AllowGet);
                }
            }
            ModelState.Remove("SMSPassword");
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            if (User.GiveUserId() != null)
            {
                SignoutUser(Request.GetOwinContext());
            }
            var result = new ServiceUtilities().AuthenticationWithPassword(login.CustomerCode, login.Password);
            if (result.ResponseMessage.ErrorCode != 0)
            {
                if (invalidCaptcha != null)
                {
                    return Json(new { invalidCaptcha = true, valid = result.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { valid = result.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            // sign in
            Session.Remove("HasCustomCaptcha");
            Utilities.CacheManager.RemoveCachekey(login.CustomerCode);
            SignInUser(result.AuthenticationWithPasswordResult.ValidDisplayName, result.AuthenticationWithPasswordResult.ID.ToString(), result.AuthenticationWithPasswordResult.SubscriberNo, result.AuthenticationWithPasswordResult.RelatedCustomers, Request.GetOwinContext());
            if (invalidCaptcha == null)
            {
                return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { valid = "", invalidCaptcha = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DirectLogin()
        {
            Session.Remove("HasCustomCaptcha");
            var googleRecaptcha = new ServiceUtilities().CustomerWebsiteGenericSettings();
            ViewBag.clientCaptchaKey = googleRecaptcha.GenericAppSettings == null ? "" : googleRecaptcha.GenericAppSettings.RecaptchaClientKey;
            if (googleRecaptcha.GenericAppSettings != null && !googleRecaptcha.GenericAppSettings.UseGoogleRecaptcha)
            {
                Session["HasCustomCaptcha"] = true;
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DirectLogin([Bind(Include = "CustomerCode")] LoginViewModel login)
        {            
            var invalidCaptcha = Session["HasCustomCaptcha"];
            if (invalidCaptcha != null)
            {
                var customCaptcha = Request.Form["customCaptcha"];
                var loginCaptcha = Session["LoginCaptcha"] as string;
                if (customCaptcha != loginCaptcha)
                {
                    return Json(new { invalidCaptcha = true, valid = CMS.Localization.Errors.InvalidCaptcha }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var googleRecaptcha = new ServiceUtilities().CustomerWebsiteGenericSettings();
                var recaptchaServerkey = googleRecaptcha.GenericAppSettings == null ? "" : googleRecaptcha.GenericAppSettings.RecaptchaServerKey;
                var captchaResponseKey = Request.Form["g-Recaptcha-Response"];
                var captcha = RezaB.Web.Captcha.GoogleRecaptchaValidator.Check(recaptchaServerkey, captchaResponseKey);
                // captcha control end
                if (captcha == RezaB.Web.Captcha.GoogleRecaptchaResultType.Fail)
                {
                    return Json(new { valid = CMS.Localization.Errors.InvalidCaptcha }, JsonRequestBehavior.AllowGet);
                }
                if (captcha == RezaB.Web.Captcha.GoogleRecaptchaResultType.NotWorking)
                {
                    return Json(new { invalidCaptcha = true, valid = CMS.Localization.Errors.InvalidCaptcha }, JsonRequestBehavior.AllowGet);
                }
            }
            ModelState.Remove("SMSPassword");
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                if (User.GiveUserId() != null)
                {
                    SignoutUser(Request.GetOwinContext());
                }
                if (login.CustomerCode.StartsWith("0"))
                    login.CustomerCode = login.CustomerCode.TrimStart('0');

                var baseRequest = new GenericServiceSettings();
                var result = client.CustomerAuthentication(new MasterISS.CustomerService.NetspeedCustomerServiceReference.CustomerServiceAuthenticationRequest()
                {
                    Culture = baseRequest.Culture,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    Hash = baseRequest.Hash,
                    AuthenticationParameters = new MasterISS.CustomerService.NetspeedCustomerServiceReference.AuthenticationRequest()
                    {
                        CustomerCode = login.CustomerCode,
                    }
                });
                if (result.ResponseMessage.ErrorCode == 0)
                {
                    if (invalidCaptcha == null)
                    {
                        return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { valid = "", invalidCaptcha = true }, JsonRequestBehavior.AllowGet);
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
        [HttpPost]
        public ActionResult CustomerLogin([Bind(Include = "CustomerCode,SMSPassword")] LoginViewModel login)
        {
            var invalidCaptcha = Session["HasCustomCaptcha"];
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                if (login.CustomerCode.StartsWith("0"))
                    login.CustomerCode = login.CustomerCode.TrimStart('0');

                var baseRequest = new GenericServiceSettings();
                var result = client.AuthenticationSMSConfirm(new MasterISS.CustomerService.NetspeedCustomerServiceReference.CustomerServiceAuthenticationSMSConfirmRequest()
                {
                    Culture = baseRequest.Culture,
                    Hash = baseRequest.Hash,
                    Rand = baseRequest.Rand,
                    Username = baseRequest.Username,
                    AuthenticationSMSConfirmParameters = new MasterISS.CustomerService.NetspeedCustomerServiceReference.AuthenticationSMSConfirmRequest()
                    {
                        CustomerCode = login.CustomerCode,
                        SMSPassword = login.SMSPassword
                    }
                });
                if (result.ResponseMessage.ErrorCode == 0)
                {
                    // sign in
                    Session.Remove("HasCustomCaptcha");
                    SignInUser(result.AuthenticationSMSConfirmResponse.ValidDisplayName, result.AuthenticationSMSConfirmResponse.ID.ToString(), result.AuthenticationSMSConfirmResponse.SubscriberNo, result.AuthenticationSMSConfirmResponse.RelatedCustomers, Request.GetOwinContext());
                    if (invalidCaptcha == null)
                    {
                        return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
                        //return Redirect(GetRedirectUrl(Request.QueryString["ReturnUrl"]));
                    }
                    return Json(new { valid = "", invalidCaptcha = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { valid = result.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { valid = CMS.Localization.Errors.SMSPasswordWrong }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetCustomCaptcha()
        {
            Session["HasCustomCaptcha"] = true;
            var img = $"<div class='form-group py-2 m-0' style='padding-top:0 !important;padding-bottom:0 !important;'><img src='{Url.Action("LoginCaptcha", "Captcha", new { id = DateTime.Now.Ticks })}' class='custom-captcha form-control h-auto font-size-h5 border-0 px-0 text-dark py-4 px-8 rounded-lg' /></div>";
            var input = $"<div class='form-group py-2  m-0'><input autocomplete='off' type='text' name='customCaptcha' id='customCaptcha' " +
                $"class='form-control h-auto font-size-h5 border-0 px-0 text-dark py-4 px-8 rounded-lg' placeholder='{CMS.Localization.Common.ValidationCode}' /></div>";
            var content = img + input;

            return Content(content);
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
            NetspeedCustomerServiceClient serviceClient = new NetspeedCustomerServiceClient();
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