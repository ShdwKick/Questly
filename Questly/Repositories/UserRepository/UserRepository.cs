using System.IdentityModel.Tokens.Jwt;
using DataModels;
using Microsoft.EntityFrameworkCore;
using Questly.DataBase;
using Questly.Helpers;
using Questly.Services;

namespace Questly.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _databaseConnection;
        private readonly ICityRepository _cityRepository;
        private readonly IAuthorizationService _authorizationService;

        public UserRepository(DatabaseContext databaseConnection,
            IAuthorizationService authorizationService, ICityRepository cityRepository)
        {
            _databaseConnection = databaseConnection;
            _authorizationService = authorizationService;
            _cityRepository = cityRepository;
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

        public async Task<string> CreateUserAsync(UserForCreate ufc)
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                Username = ufc.Username,
                Email = ufc.Email,
                PasswordHash = HashHelper.ComputeHash(ufc.Password),
                CreatedAt = DateTime.UtcNow,
            };

            if (!await _cityRepository.DoesCityExist(ufc.CityId))
                throw new Exception("City does not exist");
            user.CityId = ufc.CityId;

            var newToken = await _authorizationService.GenerateNewTokenForUser(user);

            newToken.UserId = user.Id;

            _databaseConnection.Users.Add(user);
            _databaseConnection.Authorizations.Add(newToken);

            try
            {
                await _databaseConnection.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return newToken.AuthToken;
        }

        public async Task<Authorization> TryRefreshTokenAsync(string oldToken)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(oldToken) as JwtSecurityToken;
            if (jwtToken == null) throw new ArgumentException("INVALID_TOKEN_PROBLEM");

            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (claim == null) throw new ArgumentException("INVALID_TOKEN_CLAIMS_PROBLEM");

            var user = await GetUserAsync(Guid.Parse(claim.Value));
            if (user == null) 
                throw new ArgumentException("TOKEN_GENERATION_USER_NOT_FOUND_PROBLEM");
        
            return await _authorizationService.TryRefreshTokenAsync(user, oldToken);
        }

        public async Task DropAllUsers()
        {
            await _databaseConnection.Users.ExecuteDeleteAsync();
            await _databaseConnection.SaveChangesAsync();
        }
    }
}