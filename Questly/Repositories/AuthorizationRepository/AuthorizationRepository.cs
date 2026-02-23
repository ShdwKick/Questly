using System.IdentityModel.Tokens.Jwt;
using DataModels;
using DataModels.Helpers;
using Microsoft.EntityFrameworkCore;
using Questly.DataBase;

namespace Questly.Repositories;

public class AuthorizationRepository : IAuthorizationRepository
{
    private readonly DatabaseContext _databaseConnection;
    private readonly IUserRepository _userRepository;
    private readonly ITokenHelper _tokenHelper;

    public AuthorizationRepository(DatabaseContext databaseConnection, IUserRepository userRepository, ITokenHelper tokenHelper)
    {
        _databaseConnection = databaseConnection;
        _userRepository = userRepository;
        _tokenHelper = tokenHelper;
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
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Invalid or expired refresh token")
                    .SetCode("INVALID_REFRESH_TOKEN")
                    .Build());
            
        var user = await _userRepository.GetUserByIdAsync(session.UserId);
        if (user == null)
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"User not found")
                    .SetCode("USER_NOT_FOUND")
                    .Build());

        var jti = Guid.NewGuid().ToString();
        var accessToken = new JwtSecurityTokenHandler().WriteToken(
            _tokenHelper.GenerateAccessToken(user.Id.ToString(), jti));

        return accessToken;
    }


    public async Task<TokenPair> RefreshTokens(string refreshToken, string userAgent, string ip)
    {
        var hashed = HashHelper.ComputeHash(refreshToken);

        var session = await _databaseConnection.RefreshSessions
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == hashed && s.RevokedAt == null && s.ExpiresAt > DateTime.UtcNow);

        if (session == null)
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Invalid refresh token")
                    .SetCode("INVALID_REFRESH_TOKEN")
                    .Build());

        var user = await _userRepository.GetUserByIdAsync(session.UserId);
        if (user == null)
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"User not found")
                    .SetCode("USER_NOT_FOUND")
                    .Build());

        session.RevokedAt = DateTime.UtcNow;

        var jti = Guid.NewGuid().ToString();
        var tokens = _tokenHelper.GenerateTokens(user, jti);

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
            var affectedRows = await _databaseConnection.SaveChangesAsync();
            return affectedRows > 1;
        }

        return false;
    }
}