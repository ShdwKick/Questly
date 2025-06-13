using DataModels;
using DataModels.DTOs;
using QuestlyAdmin.Repositories;
using QuestlyAdmin.Helpers;

namespace QuestlyAdmin.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger,
            IAuthorizationRepository authorizationRepository)
        {
            _userRepository = userRepository;
            _authorizationRepository = authorizationRepository;
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

            var userData = await _userRepository.GetUserByIdAsync(userId);
            if (userData == null)
                throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

            return userData;
        }

        // public async Task<TokenPair> CreateUserAsync(UserForCreate ufc, string userAgent, string ip)
        // {
        //     _logger.LogInformation($"Creating new user with: name - {ufc.Username}, email - {ufc.Email}");
        //     if (await _userRepository.DoesUserExistAsync(ufc.Username))
        //         throw new ArgumentException("EMAIL_OR_NAME_EXIST_PROBLEM");
        //
        //
        //     _logger.LogInformation($"Checked that user doesn't exist");
        //     return await _userRepository.CreateUserAsync(ufc, userAgent, ip);
        // }

        public async Task<TokenPair> LoginUser(string username, string password, string userAgent, string ip)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("INVALID_LOGIN_OR_PASSWORD_PROBLEM");

            return await _userRepository.LoginUserAsync(username, password, userAgent, ip);
        }

        public async Task<string> TryRefreshAccessTokenAsync(string refreshToken)
        {
            if (refreshToken == null || string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentNullException(nameof(refreshToken));

            return await _authorizationRepository.RefreshAccessToken(refreshToken);
        }

        public async Task<bool> Logout(string refreshToken)
        {
            return await _authorizationRepository.Logout(refreshToken);
        }

        public async Task<bool> ChangeUserBlockStatusAsync(BlockUserDTO dto)
        {
            if (dto == null || dto.UserId == null || dto.UserId == Guid.Empty)
                throw new ArgumentNullException("Invalid user data");
            return await _userRepository.ChangeUserBlockStatusAsync(dto);
        }
    }
}