using DataModels;
using Questly.Repositories;

namespace Questly.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthorizationRepository _authorizationRepository;

        public AuthorizationService(IAuthorizationRepository authorizationRepository)
        {
            _authorizationRepository = authorizationRepository;
        }

        public async Task<TokenPair> RefreshTokens(string refreshToken, string userAgent, string ip)
        {
            if(string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(userAgent) || string.IsNullOrEmpty(ip))
                throw new ArgumentNullException("Invalid refresh token or user agent or IP address");
            
            return await _authorizationRepository.RefreshTokens(refreshToken, userAgent, ip);
        }

        public Task<string> RefreshAccessToken(string refreshToken)
        {
            if(string.IsNullOrEmpty(refreshToken))
                throw new ArgumentNullException("Invalid refresh token");
            return _authorizationRepository.RefreshAccessToken(refreshToken);
        }
    }
}