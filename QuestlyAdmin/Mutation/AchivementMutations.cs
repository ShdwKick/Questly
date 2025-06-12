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

    public async Task<bool> CreateAchievement(List<AchievementDTO> achievements)
    {
        return await _achievementService.CreateAchievements(achievements);
    }
    
    public async Task<bool> UpdateAchievement(Achievement achievements)
    {
        return await _achievementService.UpdateAchievement(achievements);
    }
    
    public async Task<bool> RemoveAchievement(List<Guid> achievementsId)
    {
        return await _achievementService.RemoveAchievement(achievementsId);
    }
}