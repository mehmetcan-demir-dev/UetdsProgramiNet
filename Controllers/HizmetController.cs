using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Filters;
using UetdsProgramiNet.Models;

public class HizmetController : Controller
{
    private readonly AppDbContext _context;

    public HizmetController(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var hizmetler = await _context.Hizmetler
            .Where(h => !h.IsDeleted)  // Silinmiş olanları hariç tutuyor
            .AsNoTracking()
            .Select(r => new HizmetModel
            {
                Id = r.Id,
                IconUrl = r.IconUrl,
                Title = r.Title,
                Description = r.Description,
                InfoUrl = r.InfoUrl
            })
            .ToListAsync();

        return View(hizmetler);
    }

    public async Task<IActionResult> Bilgilendirme()
    {
        return View();
    }

    // Admin paneli için AdminIndex
    [AccessControl]
    [HttpGet]
    [Route("Hizmet/hizmet-listesi")]
    public async Task<IActionResult> AdminIndex()
    {
        var hizmetler = await _context.Hizmetler
            .Where(r => !r.IsDeleted)  // Silinmiş olanları hariç tutuyoruz
            .Select(r => new HizmetModel
            {
                Id = r.Id,
                IconUrl = r.IconUrl,
                Title = r.Title,
                Description = r.Description,
                InfoUrl = r.InfoUrl
            })
            .ToListAsync();

        return View(hizmetler);
    }

    // Referans Ekleme Sayfası - Admin
    [AccessControl]
    [Route("hizmet/hizmet-ekle")]
    public IActionResult AdminEkle()
    {
        return View();
    }

    // Referans Ekleme POST - Admin
    [AccessControl]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminEkle(HizmetModel model)
    {
        if (ModelState.IsValid)
        {
            var yeniHizmet = new Hizmet
            {
                Id = model.Id,
                IconUrl = model.IconUrl,
                Title = model.Title,
                Description = model.Description,
                InfoUrl = model.InfoUrl,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                CreatedUsername = User.Identity.Name,
                UpdatedUsername = User.Identity.Name
            };

            _context.Hizmetler.Add(yeniHizmet);
            await _context.SaveChangesAsync();

            return RedirectToAction("AdminIndex");
        }

        return View(model);
    }

    // Hizmet Güncelleme Sayfası
    [AccessControl]
    [Route("Hizmet/hizmet-guncelle")]
    public async Task<IActionResult> AdminGuncelle(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var hizmetler = await _context.Hizmetler
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted); // Silinmiş olmayanları getir
        if (hizmetler == null)
        {
            return NotFound();
        }

        var model = new HizmetModel
        {
            Id = hizmetler.Id,
            IconUrl = hizmetler.IconUrl,
            Title = hizmetler.Title,
            Description = hizmetler.Description,
            InfoUrl = hizmetler.InfoUrl
        };

        return View(model);
    }

    // Hizmet Güncelleme POST
    [AccessControl]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Hizmet/hizmet-guncelle")]
    public async Task<IActionResult> AdminGuncelle(int id, HizmetModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var hizmet = await _context.Hizmetler
                .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted); // Silinmiş olmayanları getir

            if (hizmet == null)
            {
                return NotFound();
            }

            hizmet.Description = model.Description;
            hizmet.IconUrl = model.IconUrl;
            hizmet.Title = model.Title;
            hizmet.InfoUrl = model.InfoUrl;
            hizmet.UpdatedDate = DateTime.Now;
            hizmet.UpdatedUsername = User.Identity.Name;

            try
            {
                _context.Update(hizmet);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HizmetExists(hizmet.Id))
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

    // Silme Sayfasına Yönlendiren Aksiyon
    [AccessControl]
    public async Task<IActionResult> AdminSil(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var hizmet = await _context.Hizmetler
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted); // Silinmiş olmayanları getir
        if (hizmet == null)
        {
            return NotFound();
        }

        return View(hizmet); // Silmeden önce onay ekranını göstermek için
    }

    // Silme Onayı (SilConfirmed) Aksiyon
    [AccessControl]
    [HttpPost, ActionName("Sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SilConfirmed(int id)
    {
        var hizmet = await _context.Hizmetler
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted); // Silinmiş olmayanları getir

        if (hizmet == null)
        {
            return NotFound();
        }

        hizmet.IsDeleted = true;  // Gerçekten silmek yerine "IsDeleted" değerini true yapıyoruz
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index)); // Silme işlemi tamamlandıktan sonra listeye geri dön
    }

    private bool HizmetExists(int id)
    {
        return _context.Hizmetler.Any(e => e.Id == id && !e.IsDeleted); // Silinmiş olanları kontrol etmiyoruz
    }
}
