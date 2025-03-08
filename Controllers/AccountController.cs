using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Models;

namespace UetdsProgramiNet.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // Şifremi Unuttum Sayfası
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Şifremi Unuttum Sayfasından Gelen Post İsteği
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // E-posta varsa, kullanıcıyı bilgilendirebiliriz veya yeni şifre isteği gönderebiliriz.
                    return RedirectToAction(nameof(ResetPassword), new { email = model.Email });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Bu e-posta adresi ile kayıtlı bir kullanıcı bulunmamaktadır.");
                    return View(model);
                }
            }
            return View(model);
        }

        // Şifre Sıfırlama Sayfası
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email)
        {
            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        // Şifreyi Güncelleme İşlemi
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (model.NewPassword == model.ConfirmPassword)
                    {
                        // Şifreyi hashleyip kaydediyoruz.
                        var passwordHasher = new PasswordHasher<AppUser>();
                        var hashedPassword = passwordHasher.HashPassword(user, model.NewPassword);

                        user.PasswordHash = hashedPassword;
                        var result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Şifre güncellenirken bir hata oluştu.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Şifreler uyuşmuyor.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Bu e-posta adresi ile kayıtlı bir kullanıcı bulunmamaktadır.");
                }
            }
            return View(model);
        }
    }
}
