using DataModels;
using DataModels.Extensions;
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
    IAchievementService achievementService,
    ILogger<AuthController> logger) : ControllerBase
{
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

    [HttpGet("me")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        var user = await userService.GetUserByToken();
        if (user == null)
            return NotFound(new { error = "User not found" });

        return Ok(user);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<User>> GetUserById(Guid userId)
    {
        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var user = await userService.GetUserByIdAsync(userId);
        return Ok(user);
    }

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAllUsers()
    {
        var users = userService.GetAllUsers().ToList();
        return Ok(users);
    }
}