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
                context.Result = new RedirectResult("/");
        }
    }
}
