using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet.Entities;
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
        public async Task<IActionResult> Index()
        {
            var bloglar = await _context.Bloglar
            .Select(r => new BlogModel
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                InfoUrl = r.InfoUrl,
                ImgUrl = r.ImgUrl,

            })
            .ToListAsync();

            return View(bloglar);
        }
        // Blog Ekleme Sayfası
        public IActionResult Ekle()
        {
            return View();
        }

        // Blog Ekleme POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(BlogModel model)
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
                    InfoUrl = model.InfoUrl,
                    ImgUrl = model.ImgUrl,
                    CreatedDate = DateTime.Now,  // CreatedDate'i şimdi atıyoruz
                    UpdatedDate = DateTime.Now,  // İlk güncelleme tarihini atıyoruz
                    CreatedUsername = User.Identity.Name,  // Giriş yapan kullanıcı adını alıyoruz
                    UpdatedUsername = User.Identity.Name  // Güncelleyen kullanıcıyı da aynı şekilde alıyoruz
                };

                _context.Bloglar.Add(yeniBlog);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // Blog Güncelleme Sayfası
        public async Task<IActionResult> Guncelle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloglar = await _context.Bloglar.FindAsync(id);
            if (bloglar == null)
            {
                return NotFound();
            }

            var model = new BlogModel
            {
                Id = bloglar.Id,
                Title = bloglar.Title,
                Description = bloglar.Description,
                InfoUrl = bloglar.InfoUrl,
                ImgUrl = bloglar.ImgUrl
            };

            return View(model);
        }

        // Blog Güncelleme POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(int id, BlogModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var blog = await _context.Bloglar.FindAsync(id);

                if (blog == null)
                {
                    return NotFound();
                }
                blog.Title = model.Title;
                blog.Description = model.Description;
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
            return _context.Bloglar.Any(e => e.Id == id);
        }
    }
}
