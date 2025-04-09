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

        public async Task<IActionResult> Index()
        {
            var bloglar = await _context.Bloglar
                .Where(r => r.IsDeleted == false)  // Silinmiş olanları hariç tutuyoruz
                .Select(r => new BlogModel
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    SubDescription = r.SubDescription,
                    InfoUrl = r.InfoUrl,
                    ImgUrl = r.ImgUrl,
                    PublishedDate = r.PublishedDate // Yayına alınma tarihini de alıyoruz
                })
                .ToListAsync();

            return View(bloglar);
        }

        [AccessControl]
        [HttpGet]
        [Route("Blog/blog-listesi")]
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
                    PublishedDate = r.PublishedDate // Yayına alınma tarihini de alıyoruz
                })
                .ToListAsync();

            return View(bloglar);
        }

        // Blog Ekleme Sayfası
        [AccessControl]
        [Route("Blog/blog-ekle")]
        public IActionResult AdminEkle()
        {
            return View(new BlogModel()); // Buraya model göndermezsen View'da Model null olur
        }

        // Blog Ekleme POST
        [AccessControl]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Blog/blog-ekle")]
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
                    IsDeleted = false, // Yeni eklenen kaydın silinmediğini belirtmek için false
                    PublishedDate = model.IsActive ? DateTime.Now : (DateTime?)null // Eğer aktifse yayına alınma tarihini atıyoruz
                };

                _context.Bloglar.Add(yeniBlog);
                await _context.SaveChangesAsync();

                return RedirectToAction("blog-listesi");
            }

            return View(model);
        }

        // Blog Güncelleme Sayfası
        [AccessControl]
        [Route("Blog/blog-guncelle")]
        public async Task<IActionResult> AdminGuncelle(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var bloglar = await _context.Bloglar.FindAsync(id);
            if (bloglar == null || bloglar.IsDeleted == true)
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
                ImgUrl = bloglar.ImgUrl,
                IsActive = bloglar.IsActive,
                PublishedDate = bloglar.PublishedDate // Yayına alınma tarihini ekliyoruz
            };

            return View(model);
        }

        // Blog Güncelleme POST
        [ValidateAntiForgeryToken]
        [HttpPost]
        [AccessControl]
        [Route("Blog/blog-guncelle")]
        public async Task<IActionResult> AdminGuncelle(int id, BlogModel model)
        {
            
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.ImgUrl) &&
                    !model.ImgUrl.StartsWith("/assets/images/") &&
                    !model.ImgUrl.StartsWith("http"))
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
                blog.IsActive = model.IsActive; // <-- Bu satır çok önemli
                blog.UpdatedDate = DateTime.Now;
                blog.UpdatedUsername = User.Identity.Name;

                if (model.IsActive && blog.PublishedDate == null)
                {
                    blog.PublishedDate = DateTime.Now;
                }

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

                return RedirectToAction("AdminIndex");
            }

            return View(model);
        }

        private bool BlogExists(int id)
        {
            return _context.Bloglar.Any(e => e.Id == id && e.IsDeleted == false);
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
                .Where(m => m.IsDeleted == false)
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

        // Blog Silme POST
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

            blog.IsDeleted = true;
            _context.Update(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
