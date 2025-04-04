using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UetdsProgramiNet.Filters;
using UetdsProgramiNet.Models;

namespace UetdsProgramiNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
            //HttpContext.Session.SetString("IUL", "true");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}
