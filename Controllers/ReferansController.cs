// Controllers/ReferansController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Models;

public class ReferansController : Controller
{
    private readonly AppDbContext _context;

    public ReferansController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var referanslar = await _context.Referanslar
            .Select(r => new ReferansModel
            {
                Id = r.Id,
                ImageUrl = r.ImageUrl,
                Description = r.Description,
                IsActive = r.IsActive
            })
            .ToListAsync();

        return View(referanslar);
    }

    // Referans Ekleme Sayfası
    public IActionResult Ekle()
    {
        return View();
    }

    // Referans Ekleme POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(ReferansModel model)
    {
        if (ModelState.IsValid)
        {
            var yeniReferans = new Referans
            {
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                IsActive = model.IsActive,
                CreatedUsername = User.Identity.Name  // Giriş yapan kullanıcı adını burada alıyoruz
            };

            _context.Referanslar.Add(yeniReferans);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(model);
    }


    // Referans Güncelleme Sayfası
    public async Task<IActionResult> Guncelle(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var referans = await _context.Referanslar.FindAsync(id);
        if (referans == null)
        {
            return NotFound();
        }

        // Veriyi ReferansModel'e dönüştür
        var model = new ReferansModel
        {
            Id = referans.Id,
            Description = referans.Description,
            ImageUrl = referans.ImageUrl,
            IsActive = referans.IsActive
        };

        return View(model);  // ReferansModel ile View'a gönder
    }


    // Referans Güncelleme POST
    // Referans Güncelleme POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guncelle(int id, ReferansModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var referans = await _context.Referanslar.FindAsync(id);

            if (referans == null)
            {
                return NotFound();
            }

            // Güncellenen değerleri referansa aktar
            referans.Description = model.Description;
            referans.ImageUrl = model.ImageUrl;
            referans.IsActive = model.IsActive;

            try
            {
                _context.Update(referans);  // Entity Framework ile güncelleme
                await _context.SaveChangesAsync();  // Veritabanına kaydet
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReferansExists(referans.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));  // Güncellemeyi bitirince ana sayfaya dön
        }

        return View(model);  // Hata varsa, kullanıcıya tekrar formu göster
    }

    private bool ReferansExists(int id)
    {
        return _context.Referanslar.Any(e => e.Id == id);
    }
}