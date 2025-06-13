using DataModels;
using DataModels.DTOs;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutations
{
    private readonly IUserService _userService;
    
    public UserMutations(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> ChangeUserBlockStatus(BlockUserDTO blockUser)
    {
        return await _userService.ChangeUserBlockStatusAsync(blockUser);
    }

    [GraphQLDescription("Мутация для авторизации пользователя, возвращает новый jwt токен")]
    public async Task<TokenPair> LoginUser(string login, string password, [Service] IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
        var ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
        
        return await _userService.LoginUser(login, password, userAgent, ip);
    }

}