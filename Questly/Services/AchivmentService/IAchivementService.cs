using DataModels;
using DataModels.Results;

namespace Questly.Services;

public interface IAchievementService
{
    Task<Achievement> GetAchievementInfo(Guid achId);
    Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId);
    Task<List<UserAchievement>> GetUserAchievements(Guid userId);
    Task<PaginatedResult<Achievement>> GetCityAchievementsAsync(Guid cityId, int page, int pageSize);
}
