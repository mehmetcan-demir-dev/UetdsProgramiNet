using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Filters;
using UetdsProgramiNet.Models;

namespace UetdsProgramiNet.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        public BlogController(AppDbContext context)
        {
            _context = context;
        }
        [AccessControl]
        public async Task<IActionResult> Index()
        {
            var bloglar = await _context.Bloglar
                .Where(r => r.IsDeleted == false)  // Silinmiş olanları hariç tutuyoruz (IsDeleted == false)
                .Select(r => new BlogModel
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    SubDescription = r.SubDescription,
                    InfoUrl = r.InfoUrl,
                    ImgUrl = r.ImgUrl,
                })
                .ToListAsync();

            return View(bloglar);
        }
        [AccessControl]
        [HttpGet]
        public async Task<IActionResult> AdminIndex()
        {
            var bloglar = await _context.Bloglar
                .Where(r => !r.IsDeleted)  // Silinmiş olanları hariç tutuyoruz
                .Select(r => new BlogModel
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    SubDescription = r.SubDescription,
                    InfoUrl = r.InfoUrl,
                    ImgUrl = r.ImgUrl,
                })
                .ToListAsync();

            return View(bloglar);
        }
        // Blog Ekleme Sayfası
        [AccessControl]
        public IActionResult AdminEkle()
        {
            return View();
        }

        // Blog Ekleme POST
        [AccessControl]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEkle(BlogModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.ImgUrl) && !model.ImgUrl.StartsWith("/assets/images/") && !model.ImgUrl.StartsWith("http"))
                {
                    model.ImgUrl = "/assets/images/" + model.ImgUrl;
                }

                var yeniBlog = new Blog
                {
                    Title = model.Title,
                    Description = model.Description,
                    SubDescription = model.SubDescription,
                    InfoUrl = model.InfoUrl,
                    ImgUrl = model.ImgUrl,
                    CreatedDate = DateTime.Now,  // CreatedDate'i şimdi atıyoruz
                    UpdatedDate = DateTime.Now,  // İlk güncelleme tarihini atıyoruz
                    CreatedUsername = User.Identity.Name,  // Giriş yapan kullanıcı adını alıyoruz
                    UpdatedUsername = User.Identity.Name,  // Güncelleyen kullanıcıyı da aynı şekilde alıyoruz
                    IsDeleted = false // Yeni eklenen kaydın silinmediğini belirtmek için false
                };

                _context.Bloglar.Add(yeniBlog);
                await _context.SaveChangesAsync();

                return RedirectToAction("AdminIndex");
            }

            return View(model);
        }

        // Blog Güncelleme Sayfası
        [AccessControl]
        public async Task<IActionResult> AdminGuncelle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloglar = await _context.Bloglar.FindAsync(id);
            if (bloglar == null || bloglar.IsDeleted == true)  // Silinmiş kaydı kontrol ediyoruz
            {
                return NotFound();
            }

            var model = new BlogModel
            {
                Id = bloglar.Id,
                Title = bloglar.Title,
                Description = bloglar.Description,
                SubDescription = bloglar.SubDescription,
                InfoUrl = bloglar.InfoUrl,
                ImgUrl = bloglar.ImgUrl
            };

            return View(model);
        }

        // Blog Güncelleme POST
        [AccessControl]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminGuncelle(int id, BlogModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.ImgUrl) && !model.ImgUrl.StartsWith("/assets/images/") && !model.ImgUrl.StartsWith("http"))
                {
                    model.ImgUrl = "/assets/images/" + model.ImgUrl;
                }

                var blog = await _context.Bloglar.FindAsync(id);

                if (blog == null || blog.IsDeleted == true)
                {
                    return NotFound();
                }

                blog.Title = model.Title;
                blog.Description = model.Description;
                blog.SubDescription = model.SubDescription;
                blog.InfoUrl = model.InfoUrl;
                blog.ImgUrl = model.ImgUrl;
                blog.UpdatedDate = DateTime.Now;  // Güncellenme tarihi
                blog.UpdatedUsername = User.Identity.Name;  // Güncellenen kullanıcı adı

                try
                {
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
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

        private bool BlogExists(int id)
        {
            return _context.Bloglar.Any(e => e.Id == id && e.IsDeleted == false); // Silinmiş blogları hariç tutuyoruz
        }

        // Blog Silme Sayfası
        [AccessControl]
        public async Task<IActionResult> AdminSil(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Bloglar
                .Where(m => m.IsDeleted == false)  // Silinmiş olmayanları kontrol ediyoruz
                .Select(r => new BlogModel
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description
                })
                .FirstOrDefaultAsync(m => m.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // Blog Silme POST (Silme işlemi yerine IsDeleted alanını true yapıyoruz)
        [AccessControl]
        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilConfirmed(int id)
        {
            var blog = await _context.Bloglar.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            blog.IsDeleted = true; // Kayıt silinmiş gibi işaretleniyor
            _context.Update(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
