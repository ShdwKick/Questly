using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Questly.Services;

namespace Questly.Controllers;

    [Authorize]
    [ApiController]
    [Route("api/achievements")]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementService _achievementService;

        public AchievementsController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

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
            var achievement = await _achievementService.GetAchievementInfo(achId);
            return achievement is null ? NotFound() : Ok(achievement);
        }

        /// <summary>
        /// Получить список завершенных достижений пользователя
        /// </summary>
        /// <param name="userId">Уникальный идентификатор пользователя</param>
        /// <response code="200">Список завершенных достижений успешно возвращен</response>
        [HttpGet("users/{userId}/completed")]
        [ProducesResponseType(typeof(List<UserAchievement>), 200)]
        public async Task<ActionResult<List<UserAchievement>>> GetUserCompletedAchievements(Guid userId)
        {
            var achievements = await _achievementService.GetUserCompletedAchievements(userId);
            return Ok(achievements);
        }

        /// <summary>
        /// Получить список всех достижений пользователя (включая незавершенные)
        /// </summary>
        /// <param name="userId">Уникальный идентификатор пользователя</param>
        /// <response code="200">Список достижений успешно возвращен</response>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(List<UserAchievement>), 200)]
        public async Task<ActionResult<List<UserAchievement>>> GetUserAchievements(Guid userId)
        {
            var achievements = await _achievementService.GetUserAchievements(userId);
            return Ok(achievements);
        }

        /// <summary>
        /// Получить пагинированный список достижений для города
        /// </summary>
        /// <param name="cityId">Уникальный идентификатор города</param>
        /// <param name="page">Номер страницы (начинается с 1, по умолчанию 1)</param>
        /// <param name="pageSize">Количество элементов на странице (по умолчанию 10)</param>
        /// <response code="200">Список достижений города успешно возвращен</response>
        [HttpGet("cities/{cityId:guid}")]
        [ProducesResponseType(typeof(PaginatedResult<Achievement>), 200)]
        public async Task<ActionResult<PaginatedResult<Achievement>>> GetCityAchievements(
            Guid cityId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            // Получаем данные из сервиса
            var query = _achievementService.GetCityAchievements(cityId);
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

        /// <summary>
        /// Вспомогательный класс для пагинации
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        public class PaginatedResult<T>
        {
            /// <summary>
            /// Элементы текущей страницы
            /// </summary>
            public List<T> Items { get; set; } = new List<T>();
            
            /// <summary>
            /// Общее количество элементов
            /// </summary>
            public int TotalItems { get; set; }
            
            /// <summary>
            /// Номер текущей страницы
            /// </summary>
            public int Page { get; set; }
            
            /// <summary>
            /// Размер страницы
            /// </summary>
            public int PageSize { get; set; }
        }
    }