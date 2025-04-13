using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using UetdsProgramiNet;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Filters;
using UetdsProgramiNet.Models;

public class ReferansController : Controller
{
    private readonly AppDbContext _context;

    public ReferansController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    // Admin paneli için AdminIndex
    [AccessControl]
    [HttpGet]
    [Route("Referans/referans-listesi")]
    public async Task<IActionResult> AdminIndex()
    {
        var referanslar = await _context.Referanslar
            .Where(r => !r.IsDeleted)  // Silinmiş olanları hariç tutuyoruz
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

    // Referans Ekleme Sayfası - Admin
    [AccessControl]
    [Route("Referans/referans-ekle")]
    public IActionResult AdminEkle()
    {
        return View();
    }

    // Referans Ekleme POST - Admin
    [AccessControl]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Referans/referans-ekle")]
    public async Task<IActionResult> AdminEkle(ReferansModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.Dosya != null && model.Dosya.ContentType != "image/png" && model.Dosya.ContentType != "image/jpg" && model.Dosya.ContentType != "image/jpeg")
            {
                TempData["AlertType"] = "warning";
                TempData["AlertHeader"] = "Sistem Uyarısı";
                TempData["AlertContent"] = "Geçersiz dosya tipi yüklediniz. Dosyalarınız Png, Jpg, Jpeg uzantılı olmalıdır.";
            }
            else
            {
                var fileName = Guid.NewGuid().ToString() + "_" + model.Dosya.FileName;
                var fullFilePath = Path.Combine(Environment.CurrentDirectory + $"\\wwwroot\\assets2\\img\\referanslar", fileName);
                var yeniReferans = new Referans
                {
                    Description = model.Description,
                    ImageUrl = Path.Combine($"\\assets2\\img\\referanslar", fileName),
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now,
                    CreatedUsername = HttpContext.Session.GetString("Username")
                };
                if (model.Dosya.Length > 0)
                {
                    using (var stream = System.IO.File.Create(fullFilePath))
                    {
                        await model.Dosya.CopyToAsync(stream);
                    }
                }
                _context.Referanslar.Add(yeniReferans);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminIndex");
        }

        return View(model);
    }

    // Referans Güncelleme Sayfası - Admin
    [AccessControl]
    [Route("Referans/referans-guncelle")]
    public async Task<IActionResult> AdminGuncelle(int? id)
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

        var model = new ReferansModel
        {
            Id = referans.Id,
            Description = referans.Description,
            ImageUrl = referans.ImageUrl,
            IsActive = referans.IsActive
        };

        return View(model);
    }


    // Referans Güncelleme POST - Admin
    [AccessControl]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Referans/referans-guncelle")]
    public async Task<IActionResult> AdminGuncelle(int id, ReferansModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var referans = await _context.Referanslar.FindAsync(id);

            if (referans == null || referans.IsDeleted == true)
            {
                return NotFound();
            }

            // Formdan gelen yeni açıklamayı ve durumu güncelle
            referans.Description = model.Description;
            referans.IsActive = model.IsActive;
            referans.UpdatedDate = DateTime.Now;
            referans.UpdatedUsername = User.Identity.Name;

            // Dosya yükleme işlemi
            if (model.Dosya != null)
            {
                // Geçerli dosya tiplerini kontrol et
                if (model.Dosya.ContentType != "image/png" && model.Dosya.ContentType != "image/jpg" && model.Dosya.ContentType != "image/jpeg")
                {
                    TempData["AlertType"] = "warning";
                    TempData["AlertHeader"] = "Sistem Uyarısı";
                    TempData["AlertContent"] = "Geçersiz dosya tipi yüklediniz. Dosyalarınız Png, Jpg, Jpeg uzantılı olmalıdır.";
                    return View(model);
                }

                // Yeni dosya adı oluştur
                var fileName = Guid.NewGuid().ToString() + "_" + model.Dosya.FileName;
                var fullFilePath = Path.Combine(Environment.CurrentDirectory + $"\\wwwroot\\assets2\\img\\referanslar", fileName);

                // Mevcut dosyayı sil (varsa) ve yeni dosyayı kaydet
                if (!string.IsNullOrEmpty(referans.ImageUrl))
                {
                    var oldFilePath = Path.Combine(Environment.CurrentDirectory + "\\wwwroot" + referans.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Yeni dosyayı kaydet
                using (var stream = System.IO.File.Create(fullFilePath))
                {
                    await model.Dosya.CopyToAsync(stream);
                }

                referans.ImageUrl = Path.Combine($"\\assets2\\img\\referanslar", fileName);
            }

            try
            {
                // Veritabanında güncelleme işlemi
                _context.Update(referans);
                await _context.SaveChangesAsync();
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

            return RedirectToAction("AdminIndex");
        }

        return View(model);
    }


    // Silme Sayfası - Admin
    [AccessControl]
    public async Task<IActionResult> AdminSil(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var referans = await _context.Referanslar
            .FirstOrDefaultAsync(m => m.Id == id);
        if (referans == null)
        {
            return NotFound();
        }

        return View(referans);
    }

    // Silme İşlemini Onaylama (POST) - Admin
    [AccessControl]
    [HttpPost, ActionName("AdminSil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminSilConfirmed(int id)
    {
        var referans = await _context.Referanslar.FindAsync(id);
        if (referans == null)
        {
            return NotFound();
        }

        // Veriyi silmek yerine sadece IsDeleted alanını 1 yapıyoruz
        referans.IsDeleted = true;
        referans.UpdatedDate = DateTime.Now;
        referans.UpdatedUsername = User.Identity.Name;

        _context.Referanslar.Update(referans);
        await _context.SaveChangesAsync();

        return RedirectToAction("AdminIndex");
    }

    private bool ReferansExists(int id)
    {
        return _context.Referanslar.Any(e => e.Id == id);
    }
}