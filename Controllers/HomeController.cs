using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet.Filters;
using UetdsProgramiNet.Models;

namespace UetdsProgramiNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;
        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            // Örnek: Veritabaný context'i üzerinden veri çekme (Entity Framework kullanarak)
            var blogData = _context.Bloglar.ToList(); // Blog tablosundan veriler
            var fiyatData = _context.Fiyatlar.ToList(); // Fiyat tablosundan veriler
            var hizmetData = _context.Hizmetler.ToList(); // Hizmet tablosundan veriler
            var referansData = _context.Referanslar.ToList(); // Referans tablosundan veriler
            var sliderData = _context.Sliders.ToList(); // Slider tablosundan veriler

            // Verileri ViewModel'e atama
            var viewModel = new UserViewModel
            {
                BlogData = blogData,
                FiyatData = fiyatData,
                HizmetData = hizmetData,
                ReferansData = referansData,
                SliderData = sliderData
            };

            // Veriyi View'a gönderme
            return View(viewModel);
        }
        [Route("Anasayfa/Hakkimizda")]
        public IActionResult Hakkimizda()
        {
            return View();
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
