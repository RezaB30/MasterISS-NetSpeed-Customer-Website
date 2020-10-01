﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Supports
{
    public class SupportTypes
    {

    }
    public enum ChangeType  // son değişikliği yapan
    {
        Customer = 1,
        Agent = 2
    }
    public enum SupportState
    {
        Processing = 1,
        Complete = 2
    }
    public enum SupportMessageSenderType
    {
        Customer = 1,
        Agent = 2
    }
    public enum RequestTypes
    {
        Transfer = 1,
        Fault = 2,
        Invoice = 3,
        Tariff = 4,
        Freeze = 5,
        StaticIP = 6,
        Others = 7
    }
}
