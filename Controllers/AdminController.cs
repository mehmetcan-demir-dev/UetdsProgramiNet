using Microsoft.AspNetCore.Mvc;

namespace UetdsProgramiNet.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
