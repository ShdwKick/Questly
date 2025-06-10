using DataModels;
using HotChocolate;
using HotChocolate.Types;
using Questly.Repositories;
using Questly.Services;

namespace Questly.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutations
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    
    public UserMutations(IUserRepository userRepository, IUserService userService)
    {
        _userRepository = userRepository;
        _userService = userService;
    }
    
    [GraphQLDescription("Мутация для создания пользователя, возвращает jwt токен")]
    public async Task<string> CreateUser(UserForCreate user)
    {
        return await _userService.CreateUser(user);
    }

    [GraphQLDescription("Мутация для авторизации пользователя, возвращает новый jwt токен")]
    public async Task<string> LoginUser(string login, string password)
    {
        return await _userService.LoginUser(login, password);
    }
        
    //TODO: убрать после тестирования регистрации на клиенте
    [GraphQLDescription("Временная мутация для дропа бд пользователей :)")]
    public async Task DeleteAllUsers()
    {
        await _userRepository.DropAllUsers();
    }
}