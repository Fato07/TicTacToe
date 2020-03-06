using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public interface IUserService
    {
        Task<bool>RegisterUser(UserModel userModel);
        Task<UserModel> GetUserByEmail(string email); 
        Task UpdateUser(UserModel user); 
    }
}