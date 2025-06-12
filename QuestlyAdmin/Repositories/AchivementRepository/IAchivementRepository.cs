using DataModels;
using DataModels.DTOs;

namespace QuestlyAdmin.Repositories
{
    public interface IAchievementRepository
    {
        Task<Achievement> GetAchievementInfo(Guid achId);
        Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId);
        Task<List<UserAchievement>> GetUserAchievements(Guid userId);
        
        Task<bool> CreateAchievements(List<AchievementDTO> achievements);
        Task<bool> UpdateAchievement(Achievement achievement);
        Task<bool> RemoveAchievements(List<Guid> achievementsId);
        Task<bool> DoesAchievementExist(Guid achievementId);
        Task<bool> DoesAchievementExist(string achievementName);
    }
}