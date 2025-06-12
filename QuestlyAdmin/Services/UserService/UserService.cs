using DataModels;
using DataModels.DTOs;
using QuestlyAdmin.Repositories;
using QuestlyAdmin.Helpers;

namespace QuestlyAdmin.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        public async Task<string> LoginUser(string login, string password)
        {
            if(string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("INVALID_LOGIN_OR_PASSWORD_PROBLEM");
        
            return await _userRepository.LoginUser(login, password);
        }

        public async Task<bool> ChangeUserBlockStatusAsync(BlockUserDTO blockUser)
        {
            if (blockUser == null || blockUser.UserId == Guid.Empty)
                throw new ArgumentNullException(nameof(blockUser));
            
            if(!await _userRepository.DoesUserExistAsync(blockUser.UserId))
                throw new Exception($"User with id {blockUser.UserId} does not exist");
            
            return await _userRepository.ChangeUserBlockStatus(blockUser);
        }
        
        public async Task<Authorization> TryRefreshTokenAsync(string oldToken)
        {
            if(oldToken == null || string.IsNullOrWhiteSpace(oldToken))
                throw new ArgumentNullException(nameof(oldToken));
        
            return await _userRepository.TryRefreshTokenAsync(oldToken);
        }
    }
}