﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace CustomerManagementSystem.Utilities
{
    public static class InternalUtilities
    {
        public static IEnumerable<SubscriptionBagItem> GetSubscriptionBag(this IPrincipal user)
        {
            var identity = (ClaimsIdentity)user.Identity;
            var claim = identity.Claims.FirstOrDefault(c => c.Type == "SubscriptionBag");
            if (claim == null)
                return Enumerable.Empty<SubscriptionBagItem>();
            return claim.Value.Split(';').Select(c => c.Split(',')).Select(c => new SubscriptionBagItem()
            {
                ID = c[0],
                SubscriberNo = c[1]
            }).OrderBy(s => s.ID);
        }

        public class SubscriptionBagItem
        {
            public string ID { get; set; }
            public string SubscriberNo { get; set; }
        }
        public static string GetUserIP()
        {
            try
            {
                string VisitorsIPAddr = string.Empty;
                if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
                {
                    VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
                }
                return VisitorsIPAddr;
            }
            catch (Exception)
            {
                return "???";
            }
        }
    }
}