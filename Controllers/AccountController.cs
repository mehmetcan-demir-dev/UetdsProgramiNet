using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Models;

namespace UetdsProgramiNet.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Kullanicilar.Where(m=>m.Username == model.Email && m.Password == model.Password && !m.IsDeleted).FirstOrDefault();
            if (user != null)
            {
                // Kullanıcı login oldu, session set ediliyor
                HttpContext.Session.SetString("IUL", "true");
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.AdSoyad);
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Admin");
            }

            ModelState.AddModelError("", "Geçersiz giriş denemesi.");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("IUL");
            return RedirectToAction("Login", "Account");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
    }
}
