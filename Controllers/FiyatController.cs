using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Filters;
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
                .Where(r => r.IsDeleted == false)  // Silinmiş olanları hariç tutuyoruz (IsDeleted == false)
                .Select(r => new FiyatModel
                {
                    Id = r.Id,
                    AracPaketi = r.AracPaketi,
                    KullaniciMiktari = r.KullaniciMiktari,
                    MobilBilgisi = r.MobilBilgisi,
                    DestekBilgisi = r.DestekBilgisi,
                    DestekSaatleri = r.DestekSaatleri,
                    YedeklemeTuru = r.YedeklemeTuru
                })
                .ToListAsync();

            return View(fiyatlar);
        }
        // Admin paneli için AdminIndex
        [AccessControl]
        [HttpGet]
        public async Task<IActionResult> AdminIndex()
        {
            var fiyatlar = await _context.Fiyatlar
                .Where(r => !r.IsDeleted)  // Silinmiş olanları hariç tutuyoruz
                .Select(r => new FiyatModel
                {
                    Id = r.Id,
                    AracPaketi = r.AracPaketi,
                    KullaniciMiktari = r.KullaniciMiktari,
                    MobilBilgisi = r.MobilBilgisi,
                    DestekBilgisi = r.DestekBilgisi,
                    DestekSaatleri = r.DestekSaatleri,
                    YedeklemeTuru = r.YedeklemeTuru,
                })
                .ToListAsync();

            return View(fiyatlar);
        }
        // Fiyat Ekleme Sayfası
        public IActionResult AdminEkle()
        {
            return View();
        }

        // Fiyat Ekleme POST
        [AccessControl]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEkle(FiyatModel model)
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
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatedUsername = User.Identity.Name,
                    UpdatedUsername = User.Identity.Name,
                    IsDeleted = false // Yeni eklenen kaydın silinmediğini belirtmek için false
                };

                _context.Fiyatlar.Add(yeniFiyat);
                await _context.SaveChangesAsync();

                return RedirectToAction("AdminIndex");
            }

            return View(model);
        }

        // Fiyat Güncelleme Sayfası
        [AccessControl]
        public async Task<IActionResult> AdminGuncelle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fiyatlar = await _context.Fiyatlar.FindAsync(id);
            if (fiyatlar == null || fiyatlar.IsDeleted == true)  // Silinmiş kaydı kontrol ediyoruz
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
        [AccessControl]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminGuncelle(int id, FiyatModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var fiyat = await _context.Fiyatlar.FindAsync(id);

                if (fiyat == null || fiyat.IsDeleted == true)
                {
                    return NotFound();
                }

                fiyat.AracPaketi = model.AracPaketi;
                fiyat.KullaniciMiktari = model.KullaniciMiktari;
                fiyat.MobilBilgisi = model.MobilBilgisi;
                fiyat.DestekBilgisi = model.DestekBilgisi;
                fiyat.DestekSaatleri = model.DestekSaatleri;
                fiyat.YedeklemeTuru = model.YedeklemeTuru;
                fiyat.UpdatedDate = DateTime.Now;
                fiyat.UpdatedUsername = User.Identity.Name;

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
            return _context.Fiyatlar.Any(e => e.Id == id && e.IsDeleted == false);
        }

        // Fiyat Silme Sayfası
        [AccessControl]
        public async Task<IActionResult> AdminSil(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fiyat = await _context.Fiyatlar
                .FirstOrDefaultAsync(m => m.Id == id && m.IsDeleted == false);  // Silinmiş olmayanları gösteriyoruz

            if (fiyat == null)
            {
                return NotFound();
            }

            return View(fiyat);
        }

        // Fiyat Silme POST (Silme işlemi yerine IsDeleted alanını true yapıyoruz)
        [AccessControl]
        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilConfirmed(int id)
        {
            var fiyat = await _context.Fiyatlar.FindAsync(id);
            if (fiyat == null)
            {
                return NotFound();
            }

            fiyat.IsDeleted = true; // Kayıt silinmiş gibi işaretleniyor
            _context.Update(fiyat);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
