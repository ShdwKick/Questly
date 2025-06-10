using DataModels;

namespace Questly.Services
{
    public interface IUserService
    {
        public Task<User> GetUserByToken();
        public Task<User> GetUserById(Guid userId);

        Task<string> CreateUser(UserForCreate user);
        Task<string> LoginUser(string login, string password);
    }
}