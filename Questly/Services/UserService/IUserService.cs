using DataModels;
using DataModels.Results;

namespace Questly.Services;

public interface IUserService
{
    Task<User> GetUserByToken();
    Task<User> GetUserByIdAsync(Guid userId);

    Task<TokenPair> CreateUserAsync(UserForCreate ufc, string userAgent, string ip);
    Task<TokenPair> LoginUser(string username, string password, string userAgent, string ip);
    Task<string> TryRefreshAccessTokenAsync(string refreshToken);
    Task<bool> Logout(string refreshToken);
    Task<PaginatedResult<User>> GetUsersAsync(int page, int pageSize);

    /// <summary>
    /// Выдаёт или снимает права администратора у пользователя.
    /// </summary>
    Task<User> SetAdminAsync(Guid userId, bool isAdmin);
}
