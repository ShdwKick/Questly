using DataModels;

namespace QuestlyAdmin.Services
{
    public interface IAuthorizationService
    {
        Task<Authorization> GenerateNewTokenForUser(User user);
        Task<Authorization> GetUserAuth(Guid userId);
        Task<Authorization> TryRefreshTokenAsync(User user, string oldToken);
    }
}