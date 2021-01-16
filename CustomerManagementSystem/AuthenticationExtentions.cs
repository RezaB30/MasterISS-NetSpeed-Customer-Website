using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace CustomerManagementSystem
{
    public static class AuthenticationExtentions
    {
        public static string GiveClientSubscriberNo(this IPrincipal user)
        {
            return (user.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;
        }
        public static string GiveSubscriberName(this IPrincipal user)
        {
            return (user.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
        }
        public static DateTime GiveSubscriberLastLogin(this IPrincipal user)
        {
            return Convert.ToDateTime((user.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == "LastLogin").Value);
        }
    }
}