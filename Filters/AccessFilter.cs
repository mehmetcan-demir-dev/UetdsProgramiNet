using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace UetdsProgramiNet.Filters
{
    public class AccessControlAttribute : TypeFilterAttribute
    {
        public AccessControlAttribute() : base(typeof(AccessControlFilter))
        {
        }
    }
    public class AccessControlFilter : IAuthorizationFilter
    {
        public AccessControlFilter()
        {
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string isUserLogin = context.HttpContext.Session.GetString("IUL");
            if (string.IsNullOrEmpty(isUserLogin) || isUserLogin != "true")
            {
                // Mevcut URL'i ReturnUrl olarak ekleyin
                var returnUrl = context.HttpContext.Request.Path.Value;
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
            }
            else if (context.HttpContext.Request.Path.Value == "/" || context.HttpContext.Request.Path.Value == "/Home" || context.HttpContext.Request.Path.Value == "/Home/Index")
            {
                // Ana sayfaya gelindi ve kullanıcı giriş yapmış, Referans/Index'e yönlendir
                context.Result = new RedirectToActionResult("Index", "Referans", null);
            }
        }
    }
}