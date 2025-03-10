using Microsoft.AspNetCore.Mvc;
using UetdsProgramiNet.Models;
using UetdsProgramiNet.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace UetdsProgramiNet.Controllers
{
    public class FiyatController : Controller
    {
        private readonly AppDbContext _context;

        public FiyatController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var fiyatListesi = _context.Fiyatlar.Select(f => new FiyatModel
            {
                AracSayiAciklamasi = f.AracSayiAciklamasi,
                KullaniciMiktari = f.KullaniciMiktari,
                MobilBilgisi = f.MobilBilgisi,
                DestekBilgisi = f.DestekBilgisi,
                DestekSaatleri = f.DestekSaatleri,
                YedeklemeTuru = f.YedeklemeTuru
            }).ToList();

            return View(fiyatListesi);
        }

        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ekle(FiyatModel model)
        {
            if (ModelState.IsValid)
            {
                var fiyat = new Fiyat
                {
                    AracSayiAciklamasi = model.AracSayiAciklamasi,
                    KullaniciMiktari = model.KullaniciMiktari,
                    MobilBilgisi = model.MobilBilgisi,
                    DestekBilgisi = model.DestekBilgisi,
                    DestekSaatleri = model.DestekSaatleri,
                    YedeklemeTuru = model.YedeklemeTuru,
                    CreatedDate = DateTime.Now, // CreatedDate olarak mevcut zaman
                    UpdatedDate = DateTime.Now, // İlk kaydın da güncel olmasını istiyoruz
                    CreatedUsername = User.Identity.Name // Giriş yapan kullanıcının ismi
                };

                _context.Fiyatlar.Add(fiyat);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Guncelle(int id)
        {
            var fiyat = _context.Fiyatlar.FirstOrDefault(f => f.Id == id);
            if (fiyat == null)
            {
                return NotFound();
            }

            var model = new FiyatModel
            {
                Id = fiyat.Id,
                AracSayiAciklamasi = fiyat.AracSayiAciklamasi,
                KullaniciMiktari = fiyat.KullaniciMiktari,
                MobilBilgisi = fiyat.MobilBilgisi,
                DestekBilgisi = fiyat.DestekBilgisi,
                DestekSaatleri = fiyat.DestekSaatleri,
                YedeklemeTuru = fiyat.YedeklemeTuru
            };

            return View(model);
        }
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

                fiyat.AracSayiAciklamasi = model.AracSayiAciklamasi;
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
