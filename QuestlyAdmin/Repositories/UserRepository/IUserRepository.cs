using DataModels;
using DataModels.DTOs;

// ReSharper disable All
namespace QuestlyAdmin.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(Guid userId);
        Task<bool> DoesUserExistAsync(string name);
        Task<bool> DoesUserExistAsync(Guid userId);
        Task<TokenPair> LoginUserAsync(string username, string password, string userAgent, string ip);
        //Task<TokenPair> CreateUserAsync(UserForCreate ufc, string userAgent, string ip);
        Task<bool> ChangeUserBlockStatusAsync(BlockUserDTO dto);
        
        
        //TODO: УДОЛИ
        Task DropAllUsers();
    }
}