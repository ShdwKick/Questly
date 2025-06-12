using System.IdentityModel.Tokens.Jwt;
using DataModels;
using DataModels.DTOs;
using Microsoft.EntityFrameworkCore;
using QuestlyAdmin.DataBase;
using QuestlyAdmin.Helpers;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _databaseConnection;
        private readonly IAuthorizationService _authorizationService;

        public UserRepository(DatabaseContext databaseConnection,
            IAuthorizationService authorizationService)
        {
            _databaseConnection = databaseConnection;
            _authorizationService = authorizationService;
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            var user = await _databaseConnection.Users
                .FirstOrDefaultAsync(q => q.Id == userId);

            if (user == null)
                throw new Exception($"User with id {userId} not found");

            return user;
        }

        public async Task<bool> DoesUserExistAsync(string name)
        {
            return await _databaseConnection.Users.AnyAsync(q => q.Username.Equals(name));
        }

        public async Task<bool> DoesUserExistAsync(Guid userId)
        {
            return await _databaseConnection.Users.AnyAsync(q => q.Id == userId);
        }

        public async Task<string> LoginUser(string username, string password)
        {
            var user = _databaseConnection.Users
                .FirstOrDefault(q => q.Username.Equals(username) && q.PasswordHash.Equals(HashHelper.ComputeHash(password)));

            if(user == null)
                throw new Exception("Invalid username or password");
            
            if (!user.IsAdmin)
                throw new UnauthorizedAccessException("You are not an admin :(");
        
            var auth = await _databaseConnection.Authorizations.FirstOrDefaultAsync(q=>q.UserId.Equals(user.Id));
            if(auth == null)
                throw new Exception("Invalid username or password");

            return auth.AuthToken;
        }

        public async Task<bool> ChangeUserBlockStatus(BlockUserDTO blockUser)
        {
            var user = await _databaseConnection.Users.FindAsync(blockUser.UserId);
            
            if(user!.IsBlocked == blockUser.BlockStatus)
                throw new Exception($"User with id {blockUser.UserId} already has same status");
            
            _databaseConnection.BlockUserHistory.Add(new BlockUser
            {
                Id = Guid.NewGuid(),
                UserId = blockUser.UserId,
                Reason = blockUser.Reason,
                BlockStatus = blockUser.BlockStatus,
                ModifDateTime = DateTime.UtcNow
            });
            var affectedRows = await _databaseConnection.SaveChangesAsync();

            return affectedRows > 0;
        }
        
        public async Task<Authorization> TryRefreshTokenAsync(string oldToken)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(oldToken) as JwtSecurityToken;
            if (jwtToken == null) 
                throw new ArgumentNullException("Invalid old token");

            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (claim == null) 
                throw new ArgumentNullException("Invalid token claims");

            var user = await GetUserAsync(Guid.Parse(claim.Value));
            if (user == null) 
                throw new ArgumentNullException("User with this token doesn`t exist");
        
            return await _authorizationService.TryRefreshTokenAsync(user, oldToken);
        }


        //TODO: убрать
        public async Task DropAllUsers()
        {
            await _databaseConnection.Users.ExecuteDeleteAsync();
            await _databaseConnection.SaveChangesAsync();
        }
    }
}