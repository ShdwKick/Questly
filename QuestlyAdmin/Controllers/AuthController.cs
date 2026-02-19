using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestlyAdmin.Services;
using IAuthorizationService = QuestlyAdmin.Services.IAuthorizationService;

namespace QuestlyAdmin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IUserService userService,
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthController> logger)
        : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<TokenPair>> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
                var ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

                logger.LogInformation($"Admin login attempt for user: {loginRequest.Login}");
                var tokenPair = await userService.LoginUser(loginRequest.Login, loginRequest.Password, userAgent, ip);
                return Ok(tokenPair);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning($"Admin login failed: {ex.Message}");
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenPair>> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
                var ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

                var tokenPair = await authorizationService.RefreshTokens(refreshTokenRequest.RefreshToken, userAgent, ip);
                return Ok(tokenPair);
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

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<bool>> Logout([FromBody] LogoutRequest logoutRequest)
        {
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
                var ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

                var result = await authorizationService.RefreshTokens(logoutRequest.RefreshToken, userAgent, ip);
                return Ok(new { success = result != null });
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Logout failed: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    // DTOs для запросов
    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class LogoutRequest
    {
        public string RefreshToken { get; set; }
    }
}

