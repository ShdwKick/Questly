using System.IdentityModel.Tokens.Jwt;
using DataModels;
using Microsoft.EntityFrameworkCore;
using QuestlyAdmin.DataBase;
using QuestlyAdmin.Helpers;

namespace QuestlyAdmin.Repositories
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly DatabaseContext _databaseConnection;

        public AuthorizationRepository(DatabaseContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<Authorization> GetUserAuth(Guid userId)
        {
            var auth = await _databaseConnection.Authorizations.FirstOrDefaultAsync(q=>q.UserId == userId);
        
            if(auth == null)
                auth = new Authorization
                {
                    Id = Guid.NewGuid(),
                    UserId = userId
                };
        
            return auth;
        }

        public async Task<Authorization> TryRefreshTokenAsync(User user, string oldToken)
        {
            var authorizationToken = await GetUserAuth(user.Id);
            if (authorizationToken == null) 
                throw new ArgumentException("OLD_TOKEN_NOT_FOUND_PROBLEM");

            if (HashHelper.ComputeHash(oldToken) != authorizationToken.AuthTokenHash)
                throw new ArgumentException("CORRUPTED_TOKEN_DETECTED_PROBLEM");
        
            return await GenerateNewTokenForUser(user, authorizationToken);
        }

        public async Task<Authorization> GenerateNewTokenForUser(User user, bool needRefreshId = false)
        {
            return await GenerateNewTokenForUser(user, null, needRefreshId);
        }
        public async Task<Authorization> GenerateNewTokenForUser(User user, Authorization? auth, bool needRefreshId = false)
        {
            var token = new JwtSecurityTokenHandler().WriteToken(TokenHelper.GenerateNewToken(user.Id.ToString()));

            auth ??= await GetUserAuth(user.Id);
            if (auth == null)
                throw new ArgumentException("TOKEN_GENERATION_PROBLEM");

            if(needRefreshId)
                auth.Id = Guid.NewGuid();
            auth.AuthToken = token;
            auth.AuthTokenHash = HashHelper.ComputeHash(auth.AuthToken);
            await _databaseConnection.SaveChangesAsync();
        
            return auth;
        }
    }
}