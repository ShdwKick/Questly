using DataModels;
using DataModels.Extensions;
using DataModels.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Questly.Services;

namespace Questly.Controllers;

    [Authorize]
    [ApiController]
    [Route("api/{controller}")]
    public class AchievementsController(IAchievementService achievementService) : ControllerBase
    {
        /// <summary>
        /// Получить информацию о конкретном достижении
        /// </summary>
        /// <param name="achId">Уникальный идентификатор достижения</param>
        /// <response code="200">Достижение успешно найдено</response>
        /// <response code="404">Достижение не найдено</response>
        [HttpGet("{achId:guid}")]
        [ProducesResponseType(typeof(Achievement), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Achievement>> GetAchievementInfo(Guid achId)
        {
            if(achId == Guid.Empty)
                return BadRequest("Invalid achievement ID");
            
            var achievement = await achievementService.GetAchievementInfo(achId);
            return achievement is null ? NotFound() : Ok(achievement);
        }
        
        /// <summary>
        /// Получить список завершенных достижений пользователя
        /// </summary>
        /// <param name="userId">Уникальный идентификатор пользователя</param>
        /// <response code="200">Список завершенных достижений успешно возвращен</response>
        [HttpGet("user_completed")]
        [ProducesResponseType(typeof(List<UserAchievement>), 200)]
        public async Task<ActionResult<List<UserAchievement>>> GetUserCompletedAchievements(Guid userId)
        {
            if(userId == Guid.Empty)
                return BadRequest("Invalid user ID");
            
            var achievements = await achievementService.GetUserCompletedAchievements(userId);
            return Ok(achievements);
        }


        /// <summary>
        /// Получить список всех достижений пользователя (включая незавершенные)
        /// </summary>
        /// <param name="userId">Уникальный идентификатор пользователя</param>
        /// <response code="200">Список достижений успешно возвращен</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(List<UserAchievement>), 200)]
        public async Task<ActionResult<List<UserAchievement>>> GetUserAchievements(Guid userId)
        {
            if(userId == Guid.Empty)
                return BadRequest("Invalid user ID");
            
            var achievements = await achievementService.GetUserAchievements(userId);
            return Ok(achievements);
        }

        /// <summary>
        /// Получить пагинированный список достижений для города
        /// </summary>
        /// <param name="cityId">Уникальный идентификатор города</param>
        /// <param name="page">Номер страницы (начинается с 1, по умолчанию 1)</param>
        /// <param name="pageSize">Количество элементов на странице (по умолчанию 10)</param>
        /// <response code="200">Список достижений города успешно возвращен</response>
        [HttpGet("city/{cityId:guid}")]
        [ProducesResponseType(typeof(PaginatedResult<Achievement>), 200)]
        public async Task<ActionResult<PaginatedResult<Achievement>>> GetCityAchievements(
            Guid cityId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if(cityId == Guid.Empty)
                return BadRequest("Invalid user ID");
            
            page.RequiredGreaterThan(0);
            pageSize.RequiredInRange(1, 100);

            // Получаем данные из сервиса
            var query = achievementService.GetCityAchievements(cityId);
            var totalItems = query.Count();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PaginatedResult<Achievement>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            });
        }
    }