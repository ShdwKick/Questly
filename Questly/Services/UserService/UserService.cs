using DataModels;
using DataModels.Extensions;
using DataModels.Helpers;
using DataModels.Results;
using Questly.Repositories;

namespace Questly.Services;

public class UserService(
    IUserRepository userRepository,
    IAuthorizationRepository authorizationRepository,
    ILogger<UserService> logger,
    IHeaderHelper headerHelper)
    : IUserService
{
    public async Task<User> GetUserByToken()
    {
        var userId = headerHelper.GetUserIdFromHeader();
        return await userRepository.GetUserByIdAsync(userId);
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("INVALID_USER_ID_PROBLEM");

        var userData = await userRepository.GetUserByIdAsync(userId);
        if (userData == null)
            throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

        return userData;
    }

    public async Task<TokenPair> CreateUserAsync(UserForCreate ufc, string userAgent, string ip)
    {
        logger.LogInformation($"Creating new user with: name - {ufc.Username}, email - {ufc.Email}");
        if (await userRepository.DoesUserExistAsync(ufc.Username))
            throw new ArgumentException("EMAIL_OR_NAME_EXIST_PROBLEM");


        logger.LogInformation($"Checked that user doesn't exist");
        return await userRepository.CreateUserAsync(ufc, userAgent, ip);
    }

    public async Task<TokenPair> LoginUser(string username, string password, string userAgent, string ip)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("INVALID_LOGIN_OR_PASSWORD_PROBLEM");

        return await userRepository.LoginUserAsync(username, password, userAgent, ip);
    }

    public async Task<string> TryRefreshAccessTokenAsync(string refreshToken)
    {
        if (refreshToken == null || string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentNullException(nameof(refreshToken));

        return await authorizationRepository.RefreshAccessToken(refreshToken);
    }

    public async Task<bool> Logout(string refreshToken)
    {
        return await authorizationRepository.Logout(refreshToken);
    }

    public async Task<PaginatedResult<User>> GetUsersAsync(int page, int pageSize)
    {
        page.RequiredGreaterThan(0);
        pageSize.RequiredInRange(1, 100);

        return await userRepository.GetUsersAsync(page, pageSize);
    }

    public async Task<User> SetAdminAsync(Guid userId, bool isAdmin)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("INVALID_USER_ID_PROBLEM", nameof(userId));

        // Снять права с самого себя нельзя. Иначе единственный администратор
        // одним запросом лишает систему возможности управлять контентом,
        // и вернуть права можно будет только через ADMIN_USERNAMES или SQL.
        if (!isAdmin && userId == headerHelper.GetUserIdFromHeader())
            throw new ArgumentException("CANNOT_REVOKE_OWN_ADMIN_PROBLEM", nameof(userId));

        var user = await userRepository.SetAdminAsync(userId, isAdmin);

        logger.LogInformation(
            "Права администратора у пользователя {UserId} установлены в {IsAdmin}",
            userId, isAdmin);

        return user;
    }
}
