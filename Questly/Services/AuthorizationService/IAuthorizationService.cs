using DataModels;

namespace Questly.Services
{
    public interface IAuthorizationService
    {
        Task<TokenPair> RefreshTokens(string refreshToken, string userAgent, string ip);
        Task<string> RefreshAccessToken(string refreshToken);
    }
}