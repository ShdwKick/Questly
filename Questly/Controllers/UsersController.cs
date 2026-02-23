using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questly.Services;

namespace Questly.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        try
        {
            var user = await _userService.GetUserByToken();
            if (user == null)
                return NotFound(new { error = "User not found" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting current user: {ex.Message}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<User>> GetUserById(Guid userId)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"Error getting user by ID: {ex.Message}");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting user by ID: {ex.Message}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAllUsers()
    {
        try
        {
            var users = _userService.GetAllUsers().ToList();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all users: {ex.Message}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpDelete]
    public async Task<ActionResult<bool>> DeleteAllUsers()
    {
        try
        {
            // TODO: Добавить проверку прав доступа или удалить эндпоинт после тестирования
            _logger.LogWarning("DeleteAllUsers endpoint called");
            // Для безопасности - это должен быть только тестовый эндпоинт
            // или доступный только администратору
            return Ok(new { success = true, message = "All users deleted" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting all users: {ex.Message}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}