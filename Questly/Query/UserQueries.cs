using DataModels;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Questly.Services;

namespace Questly.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class UserQueries
    {
        private readonly IUserService _userService;
    
        public UserQueries(IUserService userService)
        {
            _userService = userService;
        }
    
        [Authorize]
        [GraphQLDescription("AUTHORIZE-Получить данные о пользователе по его токену авторизации")]
        public async Task<User> GetUserByToken()
        {
            return await _userService.GetUserByToken();
        }

        [Authorize]
        [GraphQLDescription("AUTHORIZE-Получить данные о пользователе по его id")]
        public async Task<User> GetUserById(Guid userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }
        
        [Authorize]
        public async Task<bool> Logout(string token)
        {
            return await _userService.Logout(token);
        }

    }
}