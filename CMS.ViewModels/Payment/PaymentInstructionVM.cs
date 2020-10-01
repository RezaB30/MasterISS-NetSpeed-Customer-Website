using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Payment
{
    public class PaymentInstructionVM
    {
        public CreditCardStateTypes CreditCardStateTypes { get; set; }
        public string CreditCardNumber { get; set; }
    }
    public enum CreditCardStateTypes
    {
        Active = 1,
        Passive = 2
    }
}
