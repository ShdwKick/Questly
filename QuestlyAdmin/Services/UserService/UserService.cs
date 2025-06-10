using DataModels;
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
    }
}