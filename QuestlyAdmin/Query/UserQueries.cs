using DataModels;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Queries
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
            return await _userService?.GetUserById(userId);
        }

        [Authorize]
        [UsePaging]
        [UseFiltering]
        public IQueryable<User> GetAllUsers()
        {
            return _userService.GetAllUsers();
        }
    }
}