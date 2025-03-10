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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin"); // Zaten giriş yaptıysa admin paneline yönlendir
            }

            ViewData["Title"] = "Giriş Yap";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
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

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Admin"); // Giriş başarılıysa admin paneline yönlendir
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
            return RedirectToAction("Index", "Home");
        }

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
                        // Kullanıcının şifresini direkt değiştiriyoruz
                        // Token kısmı normalde email üzerinden gönderilir ama burada
                        // manuel sıfırlama yaptığımız için geçiyoruz
                        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

                        if (result.Succeeded)
                        {
                            // Başarı mesajı
                            TempData["SuccessMessage"] = "Şifreniz başarıyla güncellendi.";
                            return RedirectToAction("Login", "Account");
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
                // Aynı e-posta adresi ile kullanıcı var mı kontrol et
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Bu e-posta adresi zaten kullanılıyor.");
                    return View(model);
                }

                // Yeni kullanıcı oluştur
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    // Diğer gerekli kullanıcı bilgilerini ekleyin
                    // Örneğin: FullName = model.FullName
                };

                // Kullanıcıyı veritabanına ekle
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Başarılı kayıt sonrası otomatik giriş yap
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Ana sayfaya yönlendir
                    return RedirectToAction("Index", "Home");
                }

                // Eğer hata oluştuysa hataları ekle
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Eğer model geçerli değilse veya kayıt başarısız olduysa formu tekrar göster
            return View(model);
        }
    }
}