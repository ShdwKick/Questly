using DataModels;

namespace Questly.Repositories
{
    public interface IAchievementRepository
    {
        Task<Achievement> GetAchievementInfo(Guid achId);
        Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId);
        Task<List<UserAchievement>> GetUserAchievements(Guid userId);
        IQueryable<Achievement> GetCityAchievements(Guid cityId);
    }
}