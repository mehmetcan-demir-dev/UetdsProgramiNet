using Microsoft.AspNetCore.Mvc;
using UetdsProgramiNet;
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
                    //PublishedDate = r.PublishedDate // Yayına alınma tarihini de alıyoruz
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
                    IsActive = r.IsActive,
                    //PublishedDate = r.PublishedDate // Yayına alınma tarihini de alıyoruz
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
                // Resim URL'sini kontrol et
                if (!string.IsNullOrEmpty(model.ImgUrl) && !model.ImgUrl.StartsWith("/assets/images/") && !model.ImgUrl.StartsWith("http"))
                {
                    model.ImgUrl = "/assets/images/" + model.ImgUrl;
                }

                // Yayında olan blog için yayın tarihi atama
                var yeniBlog = new Blog
                {
                    Title = model.Title,
                    Description = model.Description,
                    SubDescription = model.SubDescription,
                    InfoUrl = model.InfoUrl,
                    ImgUrl = model.ImgUrl,
                    IsActive = model.IsActive,
                    PublishedDate = model.IsActive ? DateTime.Now : (DateTime?)null, // Eğer aktifse yayın tarihi atanır
                    CreatedUsername = User.Identity.Name // Oturumdaki kullanıcının adı, burada User.Identity.Name kullanılır
                };

                _context.Bloglar.Add(yeniBlog);
                await _context.SaveChangesAsync();

                return RedirectToAction("AdminIndex"); // Başka bir sayfaya yönlendir
            }

            return View(model); // Model geçerli değilse tekrar formu göster
        }

        // Blog Güncelleme Sayfası (GET)
        [AccessControl]
        [Route("blog/blog-guncelle")]
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
            };

            return View(model);
        }

        // Blog Güncelleme POST
        [AccessControl]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("blog/blog-guncelle")]
        public async Task<IActionResult> AdminGuncelle(BlogModel model)
        {
            if (ModelState.IsValid)
            {
                var blog = await _context.Bloglar.FindAsync(model.Id);

                if (blog != null)
                {
                    blog.Title = model.Title;
                    blog.Description = model.Description;
                    blog.SubDescription = model.SubDescription;
                    blog.InfoUrl = model.InfoUrl;
                    blog.ImgUrl = model.ImgUrl;
                    blog.IsActive = model.IsActive;

                    // Eğer aktifse, yayın tarihi güncelleniyor
                    blog.PublishedDate = model.IsActive ? DateTime.Now : blog.PublishedDate;

                    await _context.SaveChangesAsync();
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
        [HttpPost, ActionName("AdminSil")]
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

            return RedirectToAction("AdminIndex");
        }
    }
}
