using CMS.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (login.username == "123" && login.password == "123")
            {
                return Json(new { valid = "", url = "/Home/Index" }, JsonRequestBehavior.AllowGet);                
            }
            else
            {
                return Json(new { valid = "Kullanıcı Adı veya Şifre Yanlış. Tekrar Deneyin" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DirectLogin()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DirectLogin(LoginVM login)
        {
            if (string.IsNullOrEmpty(login.username))
            {
                var error = ModelState.Values.Select(m => m.Errors.Where(s => string.IsNullOrEmpty(s.ErrorMessage) == false).Select(s => s.ErrorMessage).FirstOrDefault()).FirstOrDefault();
                return Json(new { valid = error, }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CustomerLogin(string username, string password)
        {
            if (password == "1")
            {
                return Json(new { valid = "Eksik yada Hatalı Şifre" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { valid = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {
            return RedirectToAction("Login", "Account");
        }
    }
}