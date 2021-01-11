using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerManagementSystem.VPOSToken
{
    public abstract class PaymentTokenBase
    {
        public long SubscriberId { get; set; }
    }
}