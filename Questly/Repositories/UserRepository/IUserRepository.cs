using DataModels;
using DataModels.Results;

namespace Questly.Repositories;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(Guid userId);
    Task<bool> DoesUserExistAsync(string name);
    Task<bool> DoesUserExistAsync(Guid userId);
    Task<TokenPair> LoginUserAsync(string username, string password, string userAgent, string ip);
    Task<TokenPair> CreateUserAsync(UserForCreate ufc, string userAgent, string ip);

    /// <summary>
    /// Возвращает страницу пользователей. Пагинация выполняется на стороне БД.
    /// </summary>
    Task<PaginatedResult<User>> GetUsersAsync(int page, int pageSize);
}
