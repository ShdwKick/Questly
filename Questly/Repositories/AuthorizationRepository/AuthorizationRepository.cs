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

    public AuthorizationRepository(DatabaseContext databaseConnection, 
                                 IUserRepository userRepository, 
                                 ITokenHelper tokenHelper)
    {
        _databaseConnection = databaseConnection;
        _userRepository = userRepository;
        _tokenHelper = tokenHelper;
    }
        
    public async Task<string> RefreshAccessToken(string refreshToken)
    {
        // Валидация входных параметров
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));

        var hashed = HashHelper.ComputeHash(refreshToken);

        var session = await _databaseConnection.RefreshSessions
            .FirstOrDefaultAsync(s =>
                s.RefreshTokenHash == hashed &&
                s.RevokedAt == null &&
                s.ExpiresAt > DateTime.UtcNow);

        if (session == null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
            
        var user = await _userRepository.GetUserByIdAsync(session.UserId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var jti = Guid.NewGuid().ToString();
        var accessToken = new JwtSecurityTokenHandler().WriteToken(
            _tokenHelper.GenerateAccessToken(user.Id.ToString(), jti));

        return accessToken;
    }

    public async Task<TokenPair> RefreshTokens(string refreshToken, string userAgent, string ip)
    {
        // Валидация входных параметров
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));
        
        if (string.IsNullOrWhiteSpace(userAgent))
            throw new ArgumentException("User agent cannot be null or empty", nameof(userAgent));
            
        if (string.IsNullOrWhiteSpace(ip))
            throw new ArgumentException("IP address cannot be null or empty", nameof(ip));

        var hashed = HashHelper.ComputeHash(refreshToken);

        var session = await _databaseConnection.RefreshSessions
            .FirstOrDefaultAsync(s => 
                s.RefreshTokenHash == hashed && 
                s.RevokedAt == null && 
                s.ExpiresAt > DateTime.UtcNow);

        if (session == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var user = await _userRepository.GetUserByIdAsync(session.UserId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        // Отзываем старую сессию
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
        // Валидация входных параметров
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));

        var hashed = HashHelper.ComputeHash(refreshToken);

        var session = await _databaseConnection.RefreshSessions
            .FirstOrDefaultAsync(s => 
                s.RefreshTokenHash == hashed && 
                s.RevokedAt == null);

        if (session != null)
        {
            session.RevokedAt = DateTime.UtcNow;
            await _databaseConnection.SaveChangesAsync();
        }

        return false;
    }
}