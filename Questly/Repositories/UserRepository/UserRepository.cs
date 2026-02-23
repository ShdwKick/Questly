using System.IdentityModel.Tokens.Jwt;
using DataModels;
using DataModels.Helpers;
using Microsoft.EntityFrameworkCore;
using Questly.DataBase;

namespace Questly.Repositories;

public class UserRepository(
    DatabaseContext databaseConnection,
    ILogger<UserRepository> logger,
    ITokenHelper tokenHelper)
    : IUserRepository
{
    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        var user = await databaseConnection.Users
            .FirstOrDefaultAsync(q => q.Id == userId);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        return user;
    }

    public async Task<bool> DoesUserExistAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));

        return await databaseConnection.Users.AnyAsync(q => q.Username.Equals(username));
    }

    public async Task<bool> DoesUserExistAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        return await databaseConnection.Users.AnyAsync(q => q.Id == userId);
    }

    public async Task<TokenPair> LoginUserAsync(string username, string password, string userAgent, string ip)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));
            
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
            
        if (string.IsNullOrWhiteSpace(userAgent))
            throw new ArgumentException("User agent cannot be null or empty", nameof(userAgent));
            
        if (string.IsNullOrWhiteSpace(ip))
            throw new ArgumentException("IP address cannot be null or empty", nameof(ip));

        var user = await databaseConnection.Users.FirstOrDefaultAsync(q => q.Username == username);
        
        if (user == null || user.PasswordHash != HashHelper.ComputeHash(password, user.Salt))
        {
            logger.LogWarning("Failed login attempt for username: {Username}", username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var existingSession = await databaseConnection.RefreshSessions
            .FirstOrDefaultAsync(s =>
                s.UserId == user.Id &&
                s.UserAgent == userAgent &&
                s.RevokedAt == null &&
                s.ExpiresAt > DateTime.UtcNow);

        if (existingSession != null)
        {
            var jti = Guid.NewGuid().ToString();
            var accessToken = new JwtSecurityTokenHandler().WriteToken(
                tokenHelper.GenerateAccessToken(user.Id.ToString(), jti));

            return new TokenPair(accessToken, null);
        }

        var jtiNew = Guid.NewGuid().ToString();
        var tokens = tokenHelper.GenerateTokens(user, jtiNew);

        var session = new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RefreshTokenHash = HashHelper.ComputeHash(tokens.RefreshToken),
            UserAgent = userAgent,
            IpAddress = ip,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        databaseConnection.RefreshSessions.Add(session);
        await databaseConnection.SaveChangesAsync();

        return tokens;
    }

    public async Task<TokenPair> CreateUserAsync(UserForCreate ufc, string userAgent, string ip)
    {
        if (ufc == null)
            throw new ArgumentNullException(nameof(ufc), "User data cannot be null");
            
        if (string.IsNullOrWhiteSpace(ufc.Username))
            throw new ArgumentException("Username cannot be null or empty", nameof(ufc.Username));
            
        if (string.IsNullOrWhiteSpace(ufc.Email))
            throw new ArgumentException("Email cannot be null or empty", nameof(ufc.Email));
            
        if (string.IsNullOrWhiteSpace(ufc.Password))
            throw new ArgumentException("Password cannot be null or empty", nameof(ufc.Password));
            
        if (string.IsNullOrWhiteSpace(userAgent))
            throw new ArgumentException("User agent cannot be null or empty", nameof(userAgent));
            
        if (string.IsNullOrWhiteSpace(ip))
            throw new ArgumentException("IP address cannot be null or empty", nameof(ip));

        // Проверка уникальности username
        if (await DoesUserExistAsync(ufc.Username))
            throw new ArgumentException("Username is already taken", nameof(ufc.Username));

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Username = ufc.Username,
            Email = ufc.Email,
            CreatedAt = DateTime.UtcNow,
            Salt = HashHelper.GenerateSalt()
        };
        user.PasswordHash = HashHelper.ComputeHash(ufc.Password, user.Salt);

        var jti = Guid.NewGuid().ToString();
        var tokens = tokenHelper.GenerateTokens(user, jti);

        var session = new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RefreshTokenHash = HashHelper.ComputeHash(tokens.RefreshToken),
            UserAgent = userAgent,
            IpAddress = ip,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        
        databaseConnection.Users.Add(user);
        databaseConnection.RefreshSessions.Add(session);
        
        try
        {
            await databaseConnection.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("UNIQUE") == true)
                throw new ArgumentException("Email is already registered", nameof(ufc.Email));
            
            logger.LogError(ex, "Database error during user creation");
            throw new InvalidOperationException("An error occurred while creating the user", ex);
        }

        return tokens;
    }
        
    public IQueryable<User> GetAllUsers()
    {
        return databaseConnection.Users.AsQueryable();
    }
}