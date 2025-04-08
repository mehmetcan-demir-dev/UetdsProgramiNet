using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Models;
using Microsoft.EntityFrameworkCore;

namespace UetdsProgramiNet.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly AppDbContext _context;

        public UserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<bool> CreateUserAsync(CreateUserViewModel model, string role)
        {
            // AppUser (ASP.NET Identity) oluşturma
            var appUser = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(appUser, model.Password);

            if (result.Succeeded)
            {
                // Rolü kullanıcısına ekleme
                await _userManager.AddToRoleAsync(appUser, role);

                // Kullanici tablosuna da kullanıcı eklenmeli - Identity'nin ürettiği hash kullanılmalı
                var kullanici = new Kullanici
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PasswordHash = appUser.PasswordHash, // Bu satır önemli - Identity'nin oluşturduğu hash'i kullanıyoruz
                    PhoneNumber = model.PhoneNumber
                };

                _context.Kullanicilar.Add(kullanici);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<bool> UserExistsAsync(string email)
        {
            // Kullanıcı Identity ve Kullanici tablosunda kontrol edilir
            var identityUser = await _userManager.FindByEmailAsync(email);
            var customUser = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);

            return identityUser != null || customUser != null;
        }
    }
}
