using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Models;

public class HizmetController: Controller
{
    private readonly AppDbContext _context;
    public HizmetController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var hizmetler = await _context.Hizmetler
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
    // Hizmet Ekleme Sayfası
    public IActionResult Ekle()
    {
        return View();
    }

    // Hizmet Ekleme POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(HizmetModel model)
    {
        if (ModelState.IsValid)
        {
            var yeniHizmet = new Hizmet
            {
                IconUrl = model.IconUrl,
                Title = model.Title,
                Description = model.Description,
                InfoUrl = model.InfoUrl,
                CreatedDate = DateTime.Now,  // CreatedDate'i şimdi atıyoruz
                UpdatedDate = DateTime.Now,  // İlk güncelleme tarihini atıyoruz
                CreatedUsername = User.Identity.Name,  // Giriş yapan kullanıcı adını alıyoruz
                UpdatedUsername = User.Identity.Name  // Güncelleyen kullanıcıyı da aynı şekilde alıyoruz
            };

            _context.Hizmetler.Add(yeniHizmet);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(model);
    }
    // Hizmet Güncelleme Sayfası
    public async Task<IActionResult> Guncelle(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var hizmetler = await _context.Hizmetler.FindAsync(id);
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
            var hizmet = await _context.Hizmetler.FindAsync(id);

            if (hizmet == null)
            {
                return NotFound();
            }

            hizmet.Description = model.Description;
            hizmet.IconUrl = model.IconUrl;
            hizmet.Title = model.Title;
            hizmet.InfoUrl = model.InfoUrl;
            hizmet.UpdatedDate = DateTime.Now;  // Güncellenme tarihi
            hizmet.UpdatedUsername = User.Identity.Name;  // Güncellenen kullanıcı adı

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

    private bool HizmetExists(int id)
    {
        return _context.Hizmetler.Any(e => e.Id == id);
    }
}