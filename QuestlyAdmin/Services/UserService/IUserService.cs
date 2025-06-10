using DataModels;

namespace QuestlyAdmin.Services
{
    public interface IUserService
    {
        public Task<User> GetUserById(Guid userId);
        Task<string> LoginUser(string login, string password);
    }
}