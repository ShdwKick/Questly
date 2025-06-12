using DataModels;
using Questly.Repositories;
using Questly.Helpers;

namespace Questly.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> GetUserByToken()
        {
            return await UserHelper.GetUserFromHeader();
        }

        public async Task<User> GetUserById(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("INVALID_USER_ID_PROBLEM");

            var userData = await _userRepository.GetUserAsync(userId);
            if (userData == null)
                throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

            return userData;
        }

        public async Task<string> CreateUser(UserForCreate user)
        {
            _logger.LogInformation($"Creating new user with: name - {user.Username}, email - {user.Email}");
            if (await _userRepository.DoesUserExistAsync(user.Username))
                throw new ArgumentException("EMAIL_OR_NAME_EXIST_PROBLEM");
        

            _logger.LogInformation($"Checked that user doesn't exist");
            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<string> LoginUser(string login, string password)
        {
            if(string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("INVALID_LOGIN_OR_PASSWORD_PROBLEM");
        
            return await _userRepository.LoginUser(login, password);
        }
        
        public async Task<Authorization> TryRefreshTokenAsync(string oldToken)
        {
            if(oldToken == null || string.IsNullOrWhiteSpace(oldToken))
                throw new ArgumentNullException(nameof(oldToken));
        
            return await _userRepository.TryRefreshTokenAsync(oldToken);
        }
    }
}