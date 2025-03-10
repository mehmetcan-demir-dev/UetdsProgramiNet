using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Models;

namespace UetdsProgramiNet.Controllers
{
    public class FiyatController : Controller
    {
        private readonly AppDbContext _context;
        public FiyatController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var fiyatlar = await _context.Fiyatlar
            .Select(r => new FiyatModel
            {
                Id = r.Id,
                AracPaketi = r.AracPaketi,
                KullaniciMiktari = r.KullaniciMiktari,
                MobilBilgisi = r.MobilBilgisi,
                DestekBilgisi = r.DestekBilgisi,
                DestekSaatleri= r.DestekSaatleri,
                YedeklemeTuru= r.YedeklemeTuru
            })
            .ToListAsync();

            return View(fiyatlar);
        }
        // Fiyat Ekleme Sayfası
        public IActionResult Ekle()
        {
            return View();
        }

        // Fiyat Ekleme POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(FiyatModel model)
        {
            if (ModelState.IsValid)
            {
                var yeniFiyat = new Fiyat
                {
                    AracPaketi = model.AracPaketi,
                    KullaniciMiktari = model.KullaniciMiktari,
                    MobilBilgisi = model.MobilBilgisi,
                    DestekBilgisi = model.DestekBilgisi,
                    DestekSaatleri = model.DestekSaatleri,
                    YedeklemeTuru = model.YedeklemeTuru,
                    CreatedDate = DateTime.Now,  // CreatedDate'i şimdi atıyoruz
                    UpdatedDate = DateTime.Now,  // İlk güncelleme tarihini atıyoruz
                    CreatedUsername = User.Identity.Name,  // Giriş yapan kullanıcı adını alıyoruz
                    UpdatedUsername = User.Identity.Name  // Güncelleyen kullanıcıyı da aynı şekilde alıyoruz
                };

                _context.Fiyatlar.Add(yeniFiyat);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // Fiyat Güncelleme Sayfası
        public async Task<IActionResult> Guncelle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fiyatlar = await _context.Fiyatlar.FindAsync(id);
            if (fiyatlar == null)
            {
                return NotFound();
            }

            var model = new FiyatModel
            {
                Id = fiyatlar.Id,
                AracPaketi = fiyatlar.AracPaketi,
                KullaniciMiktari = fiyatlar.KullaniciMiktari,
                MobilBilgisi = fiyatlar.MobilBilgisi,
                DestekBilgisi = fiyatlar.DestekBilgisi,
                DestekSaatleri = fiyatlar.DestekSaatleri,
                YedeklemeTuru = fiyatlar.YedeklemeTuru
            };

            return View(model);
        }

        // Fiyat Güncelleme POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(int id, FiyatModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var fiyat = await _context.Fiyatlar.FindAsync(id);

                if (fiyat == null)
                {
                    return NotFound();
                }
                fiyat.AracPaketi = model.AracPaketi;
                fiyat.KullaniciMiktari = model.KullaniciMiktari;
                fiyat.MobilBilgisi = model.MobilBilgisi;
                fiyat.DestekBilgisi = model.DestekBilgisi;
                fiyat.DestekSaatleri = model.DestekSaatleri;
                fiyat.YedeklemeTuru = model.YedeklemeTuru;
                fiyat.UpdatedDate = DateTime.Now;  // Güncellenme tarihi
                fiyat.UpdatedUsername = User.Identity.Name;  // Güncellenen kullanıcı adı

                try
                {
                    _context.Update(fiyat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FiyatExists(fiyat.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        private bool FiyatExists(int id)
        {
            return _context.Fiyatlar.Any(e => e.Id == id);
        }
    }
}
