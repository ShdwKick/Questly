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

        public async Task<Authorization> GenerateNewTokenForUser(User user)
        {
            return await _authorizationRepository.GenerateNewTokenForUser(user);
        }

        public async Task<Authorization> GetUserAuth(Guid userId)
        {
            if(userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));
        
            return await _authorizationRepository.GetUserAuth(userId);
        }

        public async Task<Authorization> TryRefreshTokenAsync(User user, string oldToken)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));
            if(string.IsNullOrEmpty(oldToken))
                throw new ArgumentNullException(nameof(oldToken));
        
            return await _authorizationRepository.TryRefreshTokenAsync(user, oldToken);
        }
    }
}