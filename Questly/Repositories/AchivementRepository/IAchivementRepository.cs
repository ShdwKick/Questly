using DataModels;
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
}
