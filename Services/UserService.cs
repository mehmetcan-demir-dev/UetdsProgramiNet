using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Models;

namespace UetdsProgramiNet.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> CreateUserAsync(CreateUserViewModel model, string role)
        {
            // Rol kontrolü
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new AppRole { Name = role });
            }

            // Kullanıcı oluşturma
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return true;
            }

            return false;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
    }
}