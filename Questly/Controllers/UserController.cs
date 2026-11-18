using DataModels;
using DataModels.Requests;
using DataModels.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questly.Authorization;
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
    /// Получить пагинированный список пользователей. Только для администраторов
    /// </summary>
    /// <remarks>
    /// Ручка отдаёт email и признак администратора по каждому пользователю,
    /// то есть позволяет перечислить и аудиторию, и администраторов.
    /// Поэтому закрыта политикой Admin, а не просто авторизацией.
    /// </remarks>
    /// <param name="page">Номер страницы (начинается с 1, по умолчанию 1)</param>
    /// <param name="pageSize">Количество элементов на странице (1..100, по умолчанию 20)</param>
    /// <response code="200">Список пользователей успешно возвращен</response>
    /// <response code="400">Некорректные параметры запроса</response>
    /// <response code="403">Недостаточно прав</response>
    [Authorize(Policy = Policies.Admin)]
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedResult<User>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await userService.GetUsersAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Выдать или снять права администратора. Только для администраторов
    /// </summary>
    /// <param name="userId">Уникальный идентификатор пользователя</param>
    /// <param name="request">Новое значение признака администратора</param>
    /// <response code="200">Права изменены</response>
    /// <response code="400">Некорректные данные запроса или попытка снять права с самого себя</response>
    /// <response code="403">Недостаточно прав</response>
    /// <response code="404">Пользователь не найден</response>
    [Authorize(Policy = Policies.Admin)]
    [HttpPut("{userId:guid}/admin")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> SetUserAdmin(Guid userId, [FromBody] UserRoleRequest request)
    {
        // IsAdmin объявлен как bool? с [Required], поэтому до сюда null не доходит:
        // пустое тело отсекает валидация модели и возвращает 400.
        var user = await userService.SetAdminAsync(userId, request.IsAdmin!.Value);
        return Ok(user);
    }
}
