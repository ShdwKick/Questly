using DataModels;
using DataModels.DTOs;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class AchievementMutations
{
    private readonly IAchievementService _achievementService;
    
    public AchievementMutations(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    public async Task<bool> CreateAchievements(AchievementDTO achievement)
    {
        return await _achievementService.CreateAchievement(achievement);
    }
    
    public async Task<bool> UpdateAchievement(Achievement achievement)
    {
        return await _achievementService.UpdateAchievement(achievement);
    }
    
    public async Task<bool> RemoveAchievements(List<Guid> achievementsId)
    {
        return await _achievementService.RemoveAchievement(achievementsId);
    }
}