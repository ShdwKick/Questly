using DataModels;
using DataModels.DTOs;

namespace QuestlyAdmin.Services
{
    public interface IUserService
    {
        public Task<User> GetUserById(Guid userId);
        Task<string> LoginUser(string login, string password);
        Task<bool> ChangeUserBlockStatusAsync(BlockUserDTO blockUser);
        Task<Authorization> TryRefreshTokenAsync(string oldToken);
    }
}