using HotChocolate;
using Questly.Repositories;
using Questly.Services;

namespace Questly.Mutations
{
    public class Mutation
    {
        private readonly IUserService _userService;
        
        public Mutation(IUserService userService)        
        {
            _userService = userService;
        }


        [GraphQLDescription(
            "Если jwt токен просрочен, кидаешь его сюда и сервер пытается его обновить, " +
            "если всё хорошо то отправляет новый токен, иначе ловишь ошибку в лицо")]
        public async Task<string> TryRefreshToken(string oldToken)
        {
            var token = await _userService.TryRefreshTokenAsync(oldToken);
            return token.ToString()!;
        }
    }
}