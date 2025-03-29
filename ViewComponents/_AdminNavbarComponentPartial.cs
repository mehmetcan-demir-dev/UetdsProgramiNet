using Microsoft.AspNetCore.Mvc;

namespace UetdsProgramiNet.ViewComponents
{
    public class _AdminNavbarComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
