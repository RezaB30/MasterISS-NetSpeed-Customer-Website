using CMS.ViewModels.Payment;
using PagedList;
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
            return View(new PayBillsVM());
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult PayBill(PayBillsVM pay)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { valid = "Eksik Yada Hata Bilgi Girdiniz. Tekrar Deneyiniz" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Bills(int page = 1, int pageSize = 20) // fatura-odeme.html
        {
            var bills = new List<BillsVM>();
            var state = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                state.Add((int)BillStatusTypes.Paid);
                state.Add((int)BillStatusTypes.Paid);
                state.Add((int)BillStatusTypes.Paid);
                state.Add((int)BillStatusTypes.Unpaid);
                state.Add((int)BillStatusTypes.Paid);
            }
            for (int i = 0; i < 100; i++)
            {
                bills.Add(new BillsVM()
                {
                    BillAmount = 94.00m,
                    BillNo = new Random().Next(i, 99999),
                    BillStatusTypes = (BillStatusTypes)state[i],
                    IssueDate = DateTime.Now.AddDays(-10),
                    DueDate = DateTime.Now,
                    BeginInvoiceDate = DateTime.Now.AddDays(-(DateTime.Now.Day - 1)),
                    InvoicingPeriod = 1,
                    EndInvoiceDate = DateTime.Now
                });
            }
            var TotalCount = bills.Count();
            var list = bills.OrderByDescending(m => m.BillStatusTypes == BillStatusTypes.Unpaid).ThenByDescending(m => m.IssueDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var model = new StaticPagedList<BillsVM>(list, page, pageSize, TotalCount);
            return View(model);
        }
        public ActionResult PaymentInstruction() // otomatik-odeme.html
        {
            var list = new List<PaymentInstructionVM>();
            list.Add(new PaymentInstructionVM()
            {
                CreditCardNumber = "4444 2222 **** 1111",
                CreditCardStateTypes = CreditCardStateTypes.Passive
            });
            list.Add(new PaymentInstructionVM()
            {
                CreditCardNumber = "4444 2222 **** 1111",
                CreditCardStateTypes = CreditCardStateTypes.Passive
            });
            list.Add(new PaymentInstructionVM()
            {
                CreditCardNumber = "4444 2222 **** 1111",
                CreditCardStateTypes = CreditCardStateTypes.Active
            });
            return View(list);
        }
        public ActionResult AddNewCard() // kart-ekle.html
        {
            return View();
        }
        public ActionResult CreditCardRegister()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreditCardRegister(NewCardVM newCard)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreditCardValidation()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreditCardValidation(string SMSCode)
        {
            if (SMSCode == "1")
            {
                return Json(new { valid = "Hatalı kod girdiniz. Tekrar Deneyiniz." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RegisteredCards()
        {
            return View();
        }
    }
}