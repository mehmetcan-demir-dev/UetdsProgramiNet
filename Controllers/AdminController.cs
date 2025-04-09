using Microsoft.AspNetCore.Mvc;
using UetdsProgramiNet.Filters;

namespace UetdsProgramiNet.Controllers
{
    public class AdminController : Controller
    {
        [AccessControl]
        public IActionResult Index()
        {
            return View();
        }
    }
}
