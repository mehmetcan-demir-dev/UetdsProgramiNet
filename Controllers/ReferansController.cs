using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [AccessControl]
    [HttpGet]
    public async Task<IActionResult> Index()
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
                CreatedDate = DateTime.Now,  // CreatedDate'i şimdi atıyoruz
                UpdatedDate = DateTime.Now,  // İlk güncelleme tarihini atıyoruz
                CreatedUsername = User.Identity.Name,  // Giriş yapan kullanıcı adını alıyoruz
                UpdatedUsername = User.Identity.Name  // Güncelleyen kullanıcıyı da aynı şekilde alıyoruz
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

        var model = new ReferansModel
        {
            Id = referans.Id,
            Description = referans.Description,
            ImageUrl = referans.ImageUrl,
            IsActive = referans.IsActive
        };

        return View(model);
    }

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

            referans.Description = model.Description;
            referans.ImageUrl = model.ImageUrl;
            referans.IsActive = model.IsActive;
            referans.UpdatedDate = DateTime.Now;  // Güncellenme tarihi
            referans.UpdatedUsername = User.Identity.Name;  // Güncellenen kullanıcı adı

            try
            {
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

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // Silme Sayfası
    public async Task<IActionResult> Sil(int? id)
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

    // Silme İşlemini Onaylama (POST)
    [HttpPost, ActionName("Sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SilConfirmed(int id)
    {
        var referans = await _context.Referanslar.FindAsync(id);
        if (referans == null)
        {
            return NotFound();
        }

        // Veriyi silmek yerine sadece IsDeleted alanını 1 yapıyoruz
        referans.IsDeleted = true;
        referans.UpdatedDate = DateTime.Now;
        referans.UpdatedUsername = User.Identity.Name;  // Güncellenen kullanıcı adı

        _context.Referanslar.Update(referans);
        await _context.SaveChangesAsync();  

        return RedirectToAction(nameof(Index));
    }


    private bool ReferansExists(int id)
    {
        return _context.Referanslar.Any(e => e.Id == id);
    }
}
