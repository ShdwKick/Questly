using DataModels;

namespace Questly.Repositories
{
    public interface IAuthorizationRepository
    {
        Task<Authorization> GetUserAuth(Guid userId);
        Task<Authorization> TryRefreshTokenAsync(User user, string oldToken);
    
        Task<Authorization> GenerateNewTokenForUser(User user, bool needRefreshId = false);
        Task<Authorization> GenerateNewTokenForUser(User user, Authorization? auth, bool needRefreshId = false);
    }
}