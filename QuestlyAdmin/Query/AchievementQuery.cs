using DataModels;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class AchievementQuery
    {
        private readonly IAchievementService _achievementService;

        public AchievementQuery(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        [Authorize]
        [GraphQLDescription("AUTHORIZE-Получит информацию об ачивке по её id")]
        public async Task<Achievement> GetAchievementInfo(Guid achId)
        {
            return await _achievementService.GetAchievementInfo(achId);
        }
    
        [Authorize]
        [GraphQLDescription("AUTHORIZE-Получит информацию об ачивке по её id")]
        public async Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId)
        {
            return await _achievementService.GetUserCompletedAchievements(userId);
        }
    
        [Authorize]
        [GraphQLDescription("AUTHORIZE-Получит информацию об ачивке по её id")]
        public async Task<List<UserAchievement>> GetUserAchievements(Guid userId)
        {
            return await _achievementService.GetUserAchievements(userId);
        }

    }
}