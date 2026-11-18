using DataModels;
using DataModels.Extensions;
using DataModels.Requests;
using DataModels.Results;
using Questly.Repositories;

namespace Questly.Services;

public class AchievementService : IAchievementService
{
    private readonly IAchievementRepository _achievementRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICityRepository _cityRepository;

    public AchievementService(IAchievementRepository achievementRepository, 
                             IUserRepository userRepository,
                             ICityRepository cityRepository)
    {
        _achievementRepository = achievementRepository;
        _userRepository = userRepository;
        _cityRepository = cityRepository;
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
        
    public async Task<PaginatedResult<Achievement>> GetCityAchievementsAsync(Guid cityId, int page, int pageSize)
    {
        if (cityId == Guid.Empty)
            throw new ArgumentException("City ID cannot be empty", nameof(cityId));

        page.RequiredGreaterThan(0);
        pageSize.RequiredInRange(1, 100);

        return await _achievementRepository.GetCityAchievementsAsync(cityId, page, pageSize);
    }

    public async Task<Achievement> CreateAchievementAsync(AchievementRequest request)
    {
        request.RequiredNotNull();

        await ValidateReferencesAsync(request);

        return await _achievementRepository.CreateAchievementAsync(request);
    }

    public async Task<Achievement> UpdateAchievementAsync(Guid achId, AchievementRequest request)
    {
        request.RequiredNotNull();

        if (achId == Guid.Empty)
            throw new ArgumentException("Achievement ID cannot be empty", nameof(achId));

        await ValidateReferencesAsync(request);

        return await _achievementRepository.UpdateAchievementAsync(achId, request);
    }

    /// <summary>
    /// Проверяет, что указанные город и категория существуют. Без этой проверки
    /// битый идентификатор уходил бы в базу и возвращался нарушением внешнего
    /// ключа, то есть клиент получал бы 500 вместо внятного 404.
    /// </summary>
    private async Task ValidateReferencesAsync(AchievementRequest request)
    {
        if (request.CityId is { } cityId)
        {
            if (cityId == Guid.Empty)
                throw new ArgumentException("City ID cannot be empty", nameof(request.CityId));

            if (!await _cityRepository.DoesCityExist(cityId))
                throw new KeyNotFoundException($"City with ID {cityId} not found");
        }

        if (request.CategoryId is { } categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category ID cannot be empty", nameof(request.CategoryId));

            if (!await _achievementRepository.DoesCategoryExistAsync(categoryId))
                throw new KeyNotFoundException($"Achievement category with ID {categoryId} not found");
        }
    }
}
