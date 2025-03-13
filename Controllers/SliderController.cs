using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Models;

namespace UetdsProgramiNet.Controllers
{
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;

        public SliderController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var sliders = await _context.Sliders
                .Select(r => new SliderModel
                {
                    Id = r.Id,
                    Title = r.Title,
                    Subtitle = r.Subtitle,
                    Description = r.Description,
                    SubDescription = r.SubDescription,
                    InfoUrl = r.InfoUrl,
                    ImgUrl = r.ImgUrl
                })
                .ToListAsync();

            return View(sliders);
        }
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(SliderModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcının girdiği resim yolunu işle
                if (!string.IsNullOrEmpty(model.ImgUrl) && !model.ImgUrl.StartsWith("/assets/images/") && !model.ImgUrl.StartsWith("http"))
                {
                    model.ImgUrl = "/assets/images/" + model.ImgUrl;
                }

                var yeniSlider = new Slider
                {
                    Title = model.Title,
                    Subtitle = model.Subtitle,
                    Description = model.Description,
                    SubDescription = model.SubDescription,
                    InfoUrl = model.InfoUrl,
                    ImgUrl = model.ImgUrl,  // Güncellenmiş resim yolu
                    CreatedDate = DateTime.Now,  // İlk oluşturma tarihi
                    UpdatedDate = DateTime.Now,  // İlk güncelleme tarihi
                    CreatedUsername = User.Identity.Name,  // Giriş yapan kullanıcı
                    UpdatedUsername = User.Identity.Name   // Güncelleyen kullanıcı
                };

                _context.Sliders.Add(yeniSlider);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }


    }
}
