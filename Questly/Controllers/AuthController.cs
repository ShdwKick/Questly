using DataModels;
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
        try
        {
            var httpContext = httpContextAccessor.HttpContext;
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            var ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

            logger.LogInformation($"Login attempt for user: {loginRequest.Login}");
            var tokenPair = await userService.LoginUser(loginRequest.Login, loginRequest.Password, userAgent, ip);
            return Ok(tokenPair);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning($"Login failed: {ex.Message}");
            return Unauthorized(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Обновление access токена с использованием refresh токена
    /// </summary>
    [HttpPost("token-refresh")]
    public async Task<ActionResult<object>> RefreshAccessToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        try
        {
            var newAccessToken = await userService.TryRefreshAccessTokenAsync(refreshTokenRequest.RefreshToken);
            return Ok(new { accessToken = newAccessToken });
        }
        catch (ArgumentNullException ex)
        {
            logger.LogWarning($"Token refresh failed: {ex.Message}");
            return BadRequest(new { error = "Refresh token is required" });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning($"Token refresh failed: {ex.Message}");
            return Unauthorized(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Деактивация сессии пользователя (выход из системы)
    /// </summary>
    [Authorize]
    [HttpPost("session-logout")]
    public async Task<ActionResult<object>> LogoutUser([FromBody] LogoutRequest logoutRequest)
    {
        try
        {
            var result = await userService.Logout(logoutRequest.RefreshToken);
            if (!result)
                return NotFound(new { success = result, message = "Session not found"});
            return Ok(new { success = result, message = "Successfully logged out" });
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Logout failed: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }
}

/// <summary>
/// DTO для аутентификации
/// </summary>
public class LoginRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
}

/// <summary>
/// DTO для обновления токена
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
}

/// <summary>
/// DTO для выхода из системы
/// </summary>
public class LogoutRequest
{
    public string RefreshToken { get; set; }
}