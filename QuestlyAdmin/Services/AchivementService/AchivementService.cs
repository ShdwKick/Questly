using DataModels;
using DataModels.DTOs;
using QuestlyAdmin.Repositories;

namespace QuestlyAdmin.Services
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

        public async Task<bool> CreateAchievements(List<AchievementDTO> achievements)
        {
            if(achievements == null || achievements.Count == 0)
                throw new ArgumentNullException(nameof(achievements));

            return await _achievementRepository.CreateAchievements(achievements);
        }

        public async Task<bool> UpdateAchievement(Achievement achievement)
        {
            if(achievement == null || achievement.Id == Guid.Empty)
                throw new ArgumentNullException(nameof(achievement));
            
            if(!await _achievementRepository.DoesAchievementExist(achievement.Id))
                throw new Exception($"Achievement with id {achievement.Id} does not exist");
            
            return await _achievementRepository.UpdateAchievement(achievement);
        }

        public Task<bool> RemoveAchievement(List<Guid> achievementsId)
        {
            if(achievementsId == null || achievementsId.Count == 0)
                throw new ArgumentNullException(nameof(achievementsId));
            
            return _achievementRepository.RemoveAchievements(achievementsId);
        }

        public IQueryable<Achievement> GetCityAchievements(Guid cityId)
        {
            return _achievementRepository.GetCityAchievements(cityId);
        }
    }
}