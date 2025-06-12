using DataModels;
using DataModels.DTOs;

namespace QuestlyAdmin.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(Guid userId);
        Task<bool> DoesUserExistAsync(string name);
        Task<bool> DoesUserExistAsync(Guid userId);
        Task<string> LoginUser(string username, string password);
        Task<bool> ChangeUserBlockStatus(BlockUserDTO blockUser);
        Task<Authorization> TryRefreshTokenAsync(string oldToken);
        
        //TODO: УДОЛИ
        Task DropAllUsers();
    }
}