using System.IdentityModel.Tokens.Jwt;
using DataModels;
using Microsoft.EntityFrameworkCore;
using QuestlyAdmin.DataBase;
using QuestlyAdmin.Helpers;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _databaseConnection;

        public UserRepository(DatabaseContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
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
        
            var auth = await _databaseConnection.Authorizations.FirstOrDefaultAsync(q=>q.UserId.Equals(user.Id));
            if(auth == null)
                throw new Exception("Invalid username or password");

            return auth.AuthToken;
        }



        public async Task DropAllUsers()
        {
            await _databaseConnection.Users.ExecuteDeleteAsync();
            await _databaseConnection.SaveChangesAsync();
        }
    }
}