using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public class UserServices : IUserService
    {
        private static ConcurrentBag<UserModel> _userStore;

        static UserServices()
        {
            _userStore = new ConcurrentBag<UserModel>();
        }
        
        public Task<bool> RegisterUser(UserModel userModel)
        {
            _userStore.Add(userModel);
            return Task.FromResult(true);
        }

        public Task<UserModel> GetUserByEmail(string email)
        {
            return Task.FromResult(_userStore.FirstOrDefault(u => u.Email == email));
        }

        public Task UpdateUser(UserModel userModel)
        {
            _userStore = new ConcurrentBag<UserModel>(_userStore.Where
                (u=>u.Email != userModel.Email))
            {
                userModel
            };
            
            return Task.CompletedTask;
        }
    }
}