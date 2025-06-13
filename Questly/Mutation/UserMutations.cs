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
    private readonly ILogger<UserMutations> _logger;
    
    public UserMutations(IUserRepository userRepository, IUserService userService, ILogger<UserMutations> logger)
    {
        _userRepository = userRepository;
        _userService = userService;
        _logger = logger;
    }
    
    [GraphQLDescription("Мутация для создания пользователя, возвращает jwt токен")]
    public async Task<TokenPair> CreateUser(UserForCreate user, [Service] IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
        var ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
        
        _logger.LogInformation("Start create user");
        return await _userService.CreateUserAsync(user, userAgent, ip);
    }

    [GraphQLDescription("Мутация для авторизации пользователя, возвращает новый jwt токен")]
    public async Task<TokenPair> LoginUser(string login, string password, [Service] IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
        var ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
        
        return await _userService.LoginUser(login, password, userAgent, ip);
    }
        
    //TODO: убрать после тестирования регистрации на клиенте
    [GraphQLDescription("Временная мутация для дропа бд пользователей :)")]
    public async Task<bool> DeleteAllUsers()
    {
        await _userRepository.DropAllUsers();
        return true;
    }
}