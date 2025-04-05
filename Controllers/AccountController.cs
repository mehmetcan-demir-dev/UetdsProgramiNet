using Microsoft.AspNetCore.Authentication;
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
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Index", "Admin");
            //}

            ViewData["Title"] = "Giriş Yap";
            ViewData["ReturnUrl"] = returnUrl;

            // Hatırlanan bilgileri formda göster
            var model = new LoginViewModel();

            if (Request.Cookies.TryGetValue("RememberedEmail", out string email))
            {
                model.Email = email;
                model.RememberMe = true; // Checkbox'ı da işaretle
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["Title"] = "Giriş Yap";
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Bu e-posta adresi ile kayıtlı kullanıcı bulunamadı.");
                return View(model);
            }

            // Oturum açma işlemi - RememberMe'den bağımsız olarak 30 dakika sonra sona erecek
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false, // Tarayıcı kapatıldığında oturum kapansın
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Kesin süre
            };

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Oturum durumunu session'a kaydet
                HttpContext.Session.SetString("IUL", "true");

                // Diğer RememberMe vs. kodları

                // ReturnUrl varsa oraya yönlendir, yoksa Referans/Index'e git
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("AdminIndex", "Referans");
                }
            }

            ModelState.AddModelError(string.Empty, "Geçersiz giriş bilgileri.");
            return View(model);
        }




        // Çıkış Action'ı
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Session'ı temizle
            HttpContext.Session.Remove("IUL");

            return RedirectToAction("Login", "Account");
        }
        //BURADAN AŞAĞISI OLDUĞU GİBİ SİLİNECEK



        // Şifremi Unuttum Sayfası
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "Şifremi Unuttum";
            return View();
        }

        // Şifremi Unuttum Sayfasından Gelen Post İsteği
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ViewData["Title"] = "Şifremi Unuttum";
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
            ViewData["Title"] = "Şifre Sıfırlama";
            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        // Şifreyi Güncelleme İşlemi
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ViewData["Title"] = "Şifre Sıfırlama";
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (model.NewPassword == model.ConfirmPassword)
                    {
                        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

                        if (result.Succeeded)
                        {
                            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi. Giriş yapabilirsiniz.";
                            return RedirectToAction("Login", "Account"); // Kullanıcı login sayfasına yönlendiriliyor
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
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


        // Yardımcı metod - local URL'e yönlendirme
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // Debug için eklemeniz gereken kod
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TestLogin()
        {
            var user = await _userManager.FindByEmailAsync("test@example.com"); // Bir test e-postası kullanın
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            return Content("Kullanıcı bulunamadı.");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewData["Title"] = "Kullanıcı Oluştur";
            return View();
        }

        // Register POST Action'ı
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Bu e-posta adresi zaten kullanılıyor.");
                    return View(model);
                }

                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu. Giriş yapabilirsiniz.";
                    return RedirectToAction("Login", "Account"); // Kullanıcı giriş sayfasına yönlendiriliyor
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        
    }
}