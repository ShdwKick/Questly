using System.IdentityModel.Tokens.Jwt;
using DataModels;
using Microsoft.EntityFrameworkCore;
using Questly.DataBase;
using Questly.Helpers;
using Questly.Services;

namespace Questly.Repositories
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly DatabaseContext _databaseConnection;
        private readonly IUserRepository _userRepository;

        public AuthorizationRepository(DatabaseContext databaseConnection, IUserRepository userRepository)
        {
            _databaseConnection = databaseConnection;
            _userRepository = userRepository;
        }
        
        public async Task<string> RefreshAccessToken(string refreshToken)
        {
            var hashed = HashHelper.ComputeHash(refreshToken);

            var session = await _databaseConnection.RefreshSessions
                .FirstOrDefaultAsync(s =>
                    s.RefreshTokenHash == hashed &&
                    s.RevokedAt == null &&
                    s.ExpiresAt > DateTime.UtcNow);

            if (session == null)
                throw new Exception("Invalid or expired refresh token");
            
            var user = await _userRepository.GetUserByIdAsync(session.UserId);
            if (user == null)
                throw new Exception("User not found");

            var jti = Guid.NewGuid().ToString();
            var accessToken = new JwtSecurityTokenHandler().WriteToken(
                TokenHelper.GenerateAccessToken(user.Id.ToString(), jti));

            return accessToken;
        }


        public async Task<TokenPair> RefreshTokens(string refreshToken, string userAgent, string ip)
        {
            var hashed = HashHelper.ComputeHash(refreshToken);

            var session = await _databaseConnection.RefreshSessions
                .FirstOrDefaultAsync(s => s.RefreshTokenHash == hashed && s.RevokedAt == null && s.ExpiresAt > DateTime.UtcNow);

            if (session == null)
                throw new Exception("Invalid refresh token");

            var user = await _userRepository.GetUserByIdAsync(session.UserId);
            if (user == null)
                throw new Exception("User not found");

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