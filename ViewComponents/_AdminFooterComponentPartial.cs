using Microsoft.AspNetCore.Mvc;

namespace UetdsProgramiNet.ViewComponents
{
    public class _AdminFooterComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
