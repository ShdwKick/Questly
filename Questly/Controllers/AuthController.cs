using DataModels;
using DataModels.Extensions;
using DataModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questly.Services;

namespace Questly.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IUserService userService,
    IHttpContextAccessor httpContextAccessor,
    ILogger<AuthController> logger)
    : ControllerBase
{
    /// <summary>
    /// Аутентификация пользователя и получение токенов
    /// </summary>
    [HttpPost("session-authenticate")]
    public async Task<ActionResult<TokenPair>> AuthenticateUser([FromBody] LoginRequest loginRequest)
    {
        loginRequest.RequiredNotNull();

        var httpContext = httpContextAccessor.HttpContext;
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
        var ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

        logger.LogInformation($"Login attempt for user: {loginRequest.Login}");
        var tokenPair = await userService.LoginUser(loginRequest.Login, loginRequest.Password, userAgent, ip);
        return Ok(tokenPair);
    }
    
    
    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    [HttpPost("user_create")]
    public async Task<ActionResult<TokenPair>> RegisterUser([FromBody] UserForCreate userForCreate)
    {
        userForCreate.RequiredNotNull();

        var httpContext = httpContextAccessor.HttpContext;
        var userAgent = httpContext?.Request.Headers.UserAgent.ToString() ?? "Unknown";
        var ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

        logger.LogInformation($"Registration attempt for user: {userForCreate.Username}");
        var tokenPair = await userService.CreateUserAsync(userForCreate, userAgent, ip);
        return CreatedAtAction(nameof(RegisterUser), new { username = userForCreate.Username }, tokenPair);
    }

    /// <summary>
    /// Обновление access токена с использованием refresh токена
    /// </summary>
    [HttpPost("token-refresh")]
    public async Task<ActionResult<object>> RefreshAccessToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        refreshTokenRequest.RequiredNotNull();

        var newAccessToken = await userService.TryRefreshAccessTokenAsync(refreshTokenRequest.RefreshToken);
        return Ok(new { accessToken = newAccessToken });
    }

    /// <summary>
    /// Деактивация сессии пользователя (выход из системы)
    /// </summary>
    [Authorize]
    [HttpPost("session-logout")]
    public async Task<ActionResult<object>> LogoutUser([FromBody] LogoutRequest logoutRequest)
    {
        logoutRequest.RequiredNotNull();

        var result = await userService.Logout(logoutRequest.RefreshToken);
        if (!result)
            return NotFound(new { success = result, message = "Session not found" });
        return Ok(new { success = result, message = "Successfully logged out" });
    }
}