using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult PayBill()   //odeme.html
        {
            return View();
        }
        public ActionResult Bills() // fatura-odeme.html
        {
            return View();
        }
    }
}