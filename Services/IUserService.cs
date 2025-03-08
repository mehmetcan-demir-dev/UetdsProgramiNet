using System.Threading.Tasks;
using UetdsProgramiNet.Models;

namespace UetdsProgramiNet.Services
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(CreateUserViewModel model, string role);
        Task<bool> UserExistsAsync(string email);
    }
}