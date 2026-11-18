using DataModels;
using DataModels.Requests;
using DataModels.Results;

namespace Questly.Repositories;

public interface IAchievementRepository
{
    Task<Achievement> GetAchievementInfo(Guid achId);
    Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId);
    Task<List<UserAchievement>> GetUserAchievements(Guid userId);

    /// <summary>
    /// Возвращает страницу достижений города. Пагинация выполняется на стороне БД.
    /// </summary>
    Task<PaginatedResult<Achievement>> GetCityAchievementsAsync(Guid cityId, int page, int pageSize);

    /// <summary>
    /// Существует ли категория достижений с таким идентификатором.
    /// </summary>
    Task<bool> DoesCategoryExistAsync(Guid categoryId);

    /// <summary>
    /// Создаёт достижение и возвращает сохранённую запись с присвоенным идентификатором.
    /// </summary>
    Task<Achievement> CreateAchievementAsync(AchievementRequest request);

    /// <summary>
    /// Обновляет достижение целиком.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Достижения с таким идентификатором нет.</exception>
    Task<Achievement> UpdateAchievementAsync(Guid achId, AchievementRequest request);
}
