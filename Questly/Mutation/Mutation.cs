using HotChocolate;
using Questly.Repositories;

namespace Questly.Mutations
{
    //TODO: разнести методы по отдельным классам наследникам
    public class Mutation
    {
        private readonly IUserRepository _userRepository;
        
        public Mutation(IUserRepository userRepository)        {
            _userRepository = userRepository;
        }


        [GraphQLDescription(
            "Если jwt токен просрочен, кидаешь его сюда и сервер пытается его обновить, " +
            "если всё хорошо то отправляет новый токен, иначе ловишь ошибку в лицо")]
        public async Task<string> TryRefreshToken(string oldToken)
        {
            var token = await _userRepository.TryRefreshTokenAsync(oldToken);
            return token.ToString()!;
        }
    }
}