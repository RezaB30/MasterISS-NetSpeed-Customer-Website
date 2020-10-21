using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace CustomerManagementSystem.Auth
{
    public static class Identity
    {
        public static void IdentitySignin(UserIdentity userIdentity, string providerKey = null, bool isPersistent = false, Dictionary<int, string> roles = null)
        {
            var claims = new List<Claim>();

            // create required claims
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userIdentity.ID.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, userIdentity.Username));
            claims.Add(new Claim(ClaimTypes.Authentication, "true", ClaimValueTypes.Boolean));
            claims.Add(new Claim(ClaimTypes.UserData, userIdentity.Displayname));
            claims.Add(new Claim("ApplicationCookie", JsonConvert.SerializeObject(userIdentity)));
            //claims.Add(new Claim("AuthenticationResult", JsonConvert.SerializeObject(userIdentity.result)));
            foreach (var item in SetRoles(roles, userIdentity))
            {
                claims.Add(item);
            }
            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddHours(12)
            }, identity);
        }

        public static void IdentitySignout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                                            DefaultAuthenticationTypes.ExternalCookie);
        }

        private static IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }
        private static List<Claim> SetRoles(Dictionary<int, string> roles, UserIdentity userIdentity)
        {
            var Claims = new List<Claim>();
            Claims.Add(new Claim(ClaimTypes.Role, "All"));
            if (roles == null)
            {
                return Claims;
            }
            return Claims;
        }
    }
    public class UserIdentity
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Displayname { get; set; }
    }
}