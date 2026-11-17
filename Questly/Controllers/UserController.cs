using DataModels;
using DataModels.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questly.Services;

namespace Questly.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>
    /// Получить данные текущего пользователя (по access-токену)
    /// </summary>
    /// <response code="200">Пользователь найден</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        var user = await userService.GetUserByToken();
        if (user == null)
            return NotFound(new { error = "User not found" });

        return Ok(user);
    }

    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="userId">Уникальный идентификатор пользователя</param>
    /// <response code="200">Пользователь найден</response>
    /// <response code="400">Некорректный идентификатор</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUserById(Guid userId)
    {
        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var user = await userService.GetUserByIdAsync(userId);
        return Ok(user);
    }

    /// <summary>
    /// Получить пагинированный список пользователей
    /// </summary>
    /// <param name="page">Номер страницы (начинается с 1, по умолчанию 1)</param>
    /// <param name="pageSize">Количество элементов на странице (1..100, по умолчанию 20)</param>
    /// <response code="200">Список пользователей успешно возвращен</response>
    /// <response code="400">Некорректные параметры запроса</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedResult<User>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await userService.GetUsersAsync(page, pageSize);
        return Ok(result);
    }
}
