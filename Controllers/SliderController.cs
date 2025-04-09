using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Filters;
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

        // Ana Sayfa - Slider Listesi
        public async Task<IActionResult> Index()
        {
            var sliders = await _context.Sliders
                .Where(r => !r.IsDeleted)  // Silinmiş olanları hariç tutuyoruz
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
        // Admin paneli için AdminIndex
        [AccessControl]
        [HttpGet]
        [Route("Slider/slider-listesi")]
        public async Task<IActionResult> AdminIndex()
        {
            var sliders = await _context.Sliders
                .Where(r => !r.IsDeleted)  // Silinmiş olanları hariç tutuyoruz
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


        // Slider Ekleme Sayfası
        [AccessControl]
        [Route("Slider/slider-ekle")]
        public IActionResult AdminEkle()
        {
            return View();
        }


        // Slider Ekleme POST işlemi
        [AccessControl]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Slider/slider-ekle")]
        public async Task<IActionResult> AdminEkle(SliderModel model)
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
                    UpdatedUsername = User.Identity.Name,   // Güncelleyen kullanıcı
                    IsDeleted = false  // Varsayılan olarak silinmemiş
                };

                _context.Sliders.Add(yeniSlider);
                await _context.SaveChangesAsync();

                return RedirectToAction("AdminIndex");
            }

            return View(model);
        }

        // Slider Güncelleme Sayfası
        [AccessControl]
        [Route("Slider/slider-guncelle")]
        public async Task<IActionResult> AdminGuncelle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders
                .Where(r => !r.IsDeleted) // Silinmiş verileri hariç tutuyoruz
                .FirstOrDefaultAsync(m => m.Id == id);

            if (slider == null)
            {
                return NotFound();
            }

            var model = new SliderModel
            {
                Id = slider.Id,
                Title = slider.Title,
                Subtitle = slider.Subtitle,
                Description = slider.Description,
                SubDescription = slider.SubDescription,
                InfoUrl = slider.InfoUrl,
                ImgUrl = slider.ImgUrl
            };

            return View(model);
        }

        // Slider Güncelleme POST işlemi
        [AccessControl]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Slider/slider-guncelle")]
        public async Task<IActionResult> AdminGuncelle(int id, SliderModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var slider = await _context.Sliders.FindAsync(id);

                if (slider == null || slider.IsDeleted)
                {
                    return NotFound();
                }

                slider.Title = model.Title;
                slider.Subtitle = model.Subtitle;
                slider.Description = model.Description;
                slider.SubDescription = model.SubDescription;
                slider.InfoUrl = model.InfoUrl;
                slider.ImgUrl = model.ImgUrl; // Güncellenmiş resim yolu
                slider.UpdatedDate = DateTime.Now;  // Güncelleme tarihi
                slider.UpdatedUsername = User.Identity.Name;  // Güncellenen kullanıcı adı

                _context.Update(slider);
                await _context.SaveChangesAsync();

                return RedirectToAction("AdminIndex");
            }

            return View(model);
        }

        // Slider Silme Sayfası
        [AccessControl]
        public async Task<IActionResult> AdminSil(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders
                .Where(r => !r.IsDeleted)  // Silinmiş olanları hariç tutuyoruz
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
                .FirstOrDefaultAsync(m => m.Id == id);

            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // Slider Silme POST işlemi
        [AccessControl]
        [HttpPost, ActionName("AdminSil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilConfirmed(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }

            // Silme işlemi yerine IsDeleted alanını true yapıyoruz
            slider.IsDeleted = true;
            slider.UpdatedDate = DateTime.Now;
            slider.UpdatedUsername = User.Identity.Name;

            _context.Sliders.Update(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction("AdminIndex");
        }
    }
}
