using DataModels;
using Questly.Repositories;

namespace Questly.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _achievementRepository;
        private readonly IUserRepository _userRepository;

        public AchievementService(IAchievementRepository achievementRepository, IUserRepository userRepository)
        {
            _achievementRepository = achievementRepository;
            _userRepository = userRepository;
        }

        public async Task<Achievement> GetAchievementInfo(Guid achId)
        {
            if(Guid.Empty == achId)
                throw new ArgumentNullException(nameof(achId));

            return await _achievementRepository.GetAchievementInfo(achId);
        }

        public async Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId)
        {
            if(!await _userRepository.DoesUserExistAsync(userId))
                throw new Exception($"User with id {userId} does not exist");

            return await _achievementRepository.GetUserCompletedAchievements(userId);
        }

        public async Task<List<UserAchievement>> GetUserAchievements(Guid userId)
        {
            if(!await _userRepository.DoesUserExistAsync(userId))
                throw new Exception($"User with id {userId} does not exist");

            return await _achievementRepository.GetUserAchievements(userId);
        }
    }
}