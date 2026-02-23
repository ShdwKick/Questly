using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questly.Services;

namespace Questly.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(
    IUserService userService,
    IHttpContextAccessor httpContextAccessor,
    ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    [HttpPost("user_create")]
    public async Task<ActionResult<TokenPair>> RegisterUser([FromBody] UserForCreate userForCreate)
    {
        try
        {
            var httpContext = httpContextAccessor.HttpContext;
            var userAgent = httpContext?.Request.Headers.UserAgent.ToString() ?? "Unknown";
            var ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

            logger.LogInformation($"Registration attempt for user: {userForCreate.Username}");
            var tokenPair = await userService.CreateUserAsync(userForCreate, userAgent, ip);
            return CreatedAtAction(nameof(RegisterUser), new { username = userForCreate.Username }, tokenPair);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning($"Registration failed: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("me")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        try
        {
            var user = await userService.GetUserByToken();
            if (user == null)
                return NotFound(new { error = "User not found" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error getting current user: {ex.Message}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<User>> GetUserById(Guid userId)
    {
        try
        {
            var user = await userService.GetUserByIdAsync(userId);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning($"Error getting user by ID: {ex.Message}");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError($"Error getting user by ID: {ex.Message}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAllUsers()
    {
        try
        {
            var users = userService.GetAllUsers().ToList();
            return Ok(users);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error getting all users: {ex.Message}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}