using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerManagementSystem.VPOSToken
{
    public class QuotaSaleToken : PaymentTokenBase
    {
        public int PackageID { get; set; }
    }
}