using System.IdentityModel.Tokens.Jwt;
using DataModels;
using Microsoft.EntityFrameworkCore;
using QuestlyAdmin.Helpers;

namespace QuestlyAdmin.Repositories
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly DatabaseContext _databaseConnection;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthorizationRepository> _logger;

        public AuthorizationRepository(DatabaseContext databaseConnection, IUserRepository userRepository,
            ILogger<AuthorizationRepository> logger)
        {
            _databaseConnection = databaseConnection;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<string> RefreshAccessToken(string refreshToken)
        {
            var hashed = HashHelper.ComputeHash(refreshToken);
            _logger.LogInformation($"Hash code: {this.GetHashCode()}");
            _logger.LogInformation($"_databaseConnection Hash code: {_databaseConnection.GetHashCode()}");

            var session = await _databaseConnection.RefreshSessions
                .FirstOrDefaultAsync(s =>
                    s.RefreshTokenHash == hashed &&
                    s.RevokedAt == null &&
                    s.ExpiresAt > DateTime.UtcNow);

            if (session == null)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("Invalid or expired refresh token")
                        .SetCode("INVALID_REFRESH_TOKEN")
                        .Build());

            var user = await _userRepository.GetUserByIdAsync(session.UserId);
            if (user == null)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage($"User with id {session.UserId} was not found")
                        .SetCode("USER_NOT_FOUND")
                        .Build());

            var jti = Guid.NewGuid().ToString();
            var accessToken = new JwtSecurityTokenHandler().WriteToken(
                TokenHelper.GenerateAccessToken(user.Id.ToString(), jti));

            return accessToken;
        }


        public async Task<TokenPair> RefreshTokens(string refreshToken, string userAgent, string ip)
        {
            var hashed = HashHelper.ComputeHash(refreshToken);

            var session = await _databaseConnection.RefreshSessions
                .FirstOrDefaultAsync(s =>
                    s.RefreshTokenHash == hashed && s.RevokedAt == null && s.ExpiresAt > DateTime.UtcNow);

            if (session == null)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("Invalid or expired refresh token")
                        .SetCode("INVALID_REFRESH_TOKEN")
                        .Build());

            var user = await _userRepository.GetUserByIdAsync(session.UserId);
            if (user == null)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage($"User with id {session.UserId} was not found")
                        .SetCode("USER_NOT_FOUND")
                        .Build());

            // Revoke old session (optional, if using rotation)
            session.RevokedAt = DateTime.UtcNow;

            var jti = Guid.NewGuid().ToString();
            var tokens = TokenHelper.GenerateTokens(user, jti);

            var newSession = new RefreshSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RefreshTokenHash = HashHelper.ComputeHash(tokens.RefreshToken),
                UserAgent = userAgent,
                IpAddress = ip,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            _databaseConnection.RefreshSessions.Add(newSession);
            await _databaseConnection.SaveChangesAsync();

            return tokens;
        }

        public async Task<bool> Logout(string refreshToken)
        {
            var hashed = HashHelper.ComputeHash(refreshToken);

            var session = await _databaseConnection.RefreshSessions
                .FirstOrDefaultAsync(s => s.RefreshTokenHash == hashed && s.RevokedAt == null);

            if (session != null)
            {
                session.RevokedAt = DateTime.UtcNow;
                int affectedRows = await _databaseConnection.SaveChangesAsync();
                return affectedRows > 1;
            }

            return false;
        }
    }
}