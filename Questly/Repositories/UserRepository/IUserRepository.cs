using DataModels;

// ReSharper disable All
namespace Questly.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(Guid userId);
        Task<bool> DoesUserExistAsync(string name);
        Task<bool> DoesUserExistAsync(Guid userId);
        Task<string> LoginUser(string username, string password);
        Task<string> CreateUserAsync(UserForCreate user);
        Task<Authorization> TryRefreshTokenAsync(string oldToken);
    
    
        //TODO: УДОЛИ
        Task DropAllUsers();
    }
}