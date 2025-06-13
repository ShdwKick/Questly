using DataModels;
using DataModels.DTOs;

namespace QuestlyAdmin.Services
{
    public interface IUserService
    {
        public Task<User> GetUserByToken();
        public Task<User> GetUserById(Guid userId);

        //Task<TokenPair> CreateUserAsync(UserForCreate ufc, string userAgent, string ip);
        Task<TokenPair> LoginUser(string username, string password, string userAgent, string ip);
        Task<string> TryRefreshAccessTokenAsync(string refreshToken);
        Task<bool> Logout(string refreshToken);
        Task<bool> ChangeUserBlockStatusAsync(BlockUserDTO dto);
    }
}