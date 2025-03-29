using Microsoft.AspNetCore.Mvc;

namespace UetdsProgramiNet.ViewComponents
{
    public class _AdminSidebarComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
