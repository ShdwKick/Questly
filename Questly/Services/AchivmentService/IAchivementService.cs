using DataModels;
using DataModels.Requests;
using DataModels.Results;

namespace Questly.Services;

public interface IAchievementService
{
    Task<Achievement> GetAchievementInfo(Guid achId);
    Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId);
    Task<List<UserAchievement>> GetUserAchievements(Guid userId);
    Task<PaginatedResult<Achievement>> GetCityAchievementsAsync(Guid cityId, int page, int pageSize);

    /// <summary>
    /// Создаёт достижение. Город и категория, если указаны, должны существовать.
    /// </summary>
    Task<Achievement> CreateAchievementAsync(AchievementRequest request);

    /// <summary>
    /// Обновляет достижение целиком.
    /// </summary>
    Task<Achievement> UpdateAchievementAsync(Guid achId, AchievementRequest request);
}
