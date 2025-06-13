using DataModels;
using DataModels.DTOs;

namespace QuestlyAdmin.Services
{
    public interface IAchievementService
    {
        Task<Achievement> GetAchievementInfo(Guid achId);
        Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId);
        Task<List<UserAchievement>> GetUserAchievements(Guid userId);
        
        Task<bool> CreateAchievements(List<AchievementDTO> achievements);
        Task<bool> UpdateAchievement(Achievement achievement);
        Task<bool> RemoveAchievement(List<Guid> achievementsId);
        IQueryable<Achievement> GetCityAchievements(Guid cityId);
    }
}