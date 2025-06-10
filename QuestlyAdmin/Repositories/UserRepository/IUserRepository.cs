using DataModels;

// ReSharper disable All
namespace QuestlyAdmin.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(Guid userId);
        Task<bool> DoesUserExistAsync(string name);
        Task<bool> DoesUserExistAsync(Guid userId);
        Task<string> LoginUser(string username, string password);
    
    
        //TODO: УДОЛИ
        Task DropAllUsers();
    }
}