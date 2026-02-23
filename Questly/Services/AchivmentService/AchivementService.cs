using DataModels;
using Questly.Repositories;

namespace Questly.Services;

public class AchievementService : IAchievementService
{
    private readonly IAchievementRepository _achievementRepository;
    private readonly IUserRepository _userRepository;

    public AchievementService(IAchievementRepository achievementRepository, 
                             IUserRepository userRepository)
    {
        _achievementRepository = achievementRepository;
        _userRepository = userRepository;
    }

    public async Task<Achievement> GetAchievementInfo(Guid achId)
    {
        if (achId == Guid.Empty)
            throw new ArgumentException("Achievement ID cannot be empty", nameof(achId));

        return await _achievementRepository.GetAchievementInfo(achId);
    }

    public async Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (!await _userRepository.DoesUserExistAsync(userId))
            throw new KeyNotFoundException($"User with ID {userId} does not exist");

        return await _achievementRepository.GetUserCompletedAchievements(userId);
    }

    public async Task<List<UserAchievement>> GetUserAchievements(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (!await _userRepository.DoesUserExistAsync(userId))
            throw new KeyNotFoundException($"User with ID {userId} does not exist");

        return await _achievementRepository.GetUserAchievements(userId);
    }
        
    public IQueryable<Achievement> GetCityAchievements(Guid cityId)
    {
        if (cityId == Guid.Empty)
            throw new ArgumentException("City ID cannot be empty", nameof(cityId));
            
        return _achievementRepository.GetCityAchievements(cityId);
    }
}