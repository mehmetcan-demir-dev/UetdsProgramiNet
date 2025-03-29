using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet;
using UetdsProgramiNet.Entities;
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

    // Hizmet Güncelleme Sayfası
    public async Task<IActionResult> Guncelle(int? id)
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guncelle(int id, HizmetModel model)
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

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // Silme Sayfasına Yönlendiren Aksiyon
    public async Task<IActionResult> Sil(int? id)
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
