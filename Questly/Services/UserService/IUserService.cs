using DataModels;

namespace Questly.Services
{
    public interface IUserService
    {
        public Task<User> GetUserByToken();
        public Task<User> GetUserByIdAsync(Guid userId);

        Task<TokenPair> CreateUserAsync(UserForCreate ufc, string userAgent, string ip);
        Task<TokenPair> LoginUser(string username, string password, string userAgent, string ip);
        Task<string> TryRefreshAccessTokenAsync(string refreshToken);
        Task<bool> Logout(string refreshToken);
        IQueryable<User> GetAllUsers();
    }
}