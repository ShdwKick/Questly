using DataModels;

namespace QuestlyAdmin.Repositories
{
    public interface IAuthorizationRepository
    {
        Task<TokenPair> RefreshTokens(string refreshToken, string userAgent, string ip);
        Task<string> RefreshAccessToken(string refreshToken);
        Task<bool> Logout(string refreshToken);
    }
}