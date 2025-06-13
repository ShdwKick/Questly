using System.IdentityModel.Tokens.Jwt;
using DataModels;
using DataModels.DTOs;
using Microsoft.EntityFrameworkCore;
using QuestlyAdmin.Helpers;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _databaseConnection;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(DatabaseContext databaseConnection, ILogger<UserRepository> logger)
        {
            _databaseConnection = databaseConnection;
            _logger = logger;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _databaseConnection.Users
                .FirstOrDefaultAsync(q => q.Id == userId);

            if (user == null)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage($"User with id {userId} not found")
                        .SetCode("USER_NOT_FOUND")
                        .Build());

            return user;
        }

        public async Task<bool> DoesUserExistAsync(string name)
        {
            try
            {
                _logger.LogInformation($"Check is user exist with name {name}. in database with id {_databaseConnection.Database} and user count {_databaseConnection.Users.Count()}");
                return await _databaseConnection.Users.AnyAsync(q => q.Username.Equals(name));
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured while checking does user exist. Exception: {e}");
                throw;
            }
            
        }

        public async Task<bool> DoesUserExistAsync(Guid userId)
        {
            return await _databaseConnection.Users.AnyAsync(q => q.Id == userId);
        }

        public async Task<TokenPair> LoginUserAsync(string username, string password, string userAgent, string ip)
        {
            var user = await _databaseConnection.Users.FirstOrDefaultAsync(q => q.Username == username);
            if (user == null || user.PasswordHash != HashHelper.ComputeHash(password, user.Salt))
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("Invalid username or password")
                        .SetCode("AUTH_INVALID_CREDENTIALS")
                        .Build());
            
            if(user.IsBlocked)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage("User blocked")
                        .SetCode("USER_BLOCKED")
                        .Build());

            var existingSession = await _databaseConnection.RefreshSessions
                .FirstOrDefaultAsync(s =>
                    s.UserId == user.Id &&
                    s.UserAgent == userAgent &&
                    s.RevokedAt == null &&
                    s.ExpiresAt > DateTime.UtcNow);

            if (existingSession != null)
            {
                var jti = Guid.NewGuid().ToString();
                var accessToken = new JwtSecurityTokenHandler().WriteToken(
                    TokenHelper.GenerateAccessToken(user.Id.ToString(), jti));

                return new TokenPair(accessToken, null);
            }

            var jtiNew = Guid.NewGuid().ToString();
            var tokens = TokenHelper.GenerateTokens(user, jtiNew);

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

            _databaseConnection.RefreshSessions.Add(session);
            await _databaseConnection.SaveChangesAsync();

            return tokens;
        }

        public async Task<TokenPair> CreateUserAsync(UserForCreate ufc, string userAgent, string ip)
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                Username = ufc.Username,
                Email = ufc.Email,
                CreatedAt = DateTime.UtcNow,
                Salt = HashHelper.GenerateSalt()
            };
            user.PasswordHash = HashHelper.ComputeHash(ufc.Password + user.Salt);

            var jti = Guid.NewGuid().ToString();
            var tokens = TokenHelper.GenerateTokens(user, jti);

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
            _databaseConnection.Users.Add(user);
            _databaseConnection.RefreshSessions.Add(session);
            await _databaseConnection.SaveChangesAsync();

            return tokens;
        }

        public async Task<bool> ChangeUserBlockStatusAsync(BlockUserDTO dto)
        {
            var user = await GetUserByIdAsync(dto.UserId);
            if(user == null)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage($"User with id {dto.UserId} not found")
                        .SetCode("USER_NOT_FOUND")
                        .Build());

            if (user.IsBlocked == dto.BlockStatus)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage($"User with id {dto.UserId} already has {dto.BlockStatus} block status")
                        .SetCode("USER_BLOCK_STATUS_ALREADY_SAME")
                        .Build());
            
            user.IsBlocked = dto.BlockStatus;
            user.BlockReason = dto.Reason;

            _databaseConnection.BlockUserHistory.Add(new BlockUser
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                BlockStatus = dto.BlockStatus,
                Reason = dto.Reason,
                ModifDateTime = DateTime.UtcNow
            });
            
            int affectedRows = await _databaseConnection.SaveChangesAsync();
            return affectedRows > 0;
        }

        public IQueryable<User> GetAllUsers()
        {
            return _databaseConnection.Users.AsQueryable();
        }

        public async Task DropAllUsers()
        {
            await _databaseConnection.Users.ExecuteDeleteAsync();
            await _databaseConnection.SaveChangesAsync();
        }
    }
}