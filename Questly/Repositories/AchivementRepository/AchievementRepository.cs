using DataModels;
using DataModels.Requests;
using DataModels.Results;
using Microsoft.EntityFrameworkCore;

namespace Questly.Repositories;

public class AchievementRepository : IAchievementRepository
{
    private readonly DatabaseContext _databaseConnection;

    public AchievementRepository(DatabaseContext databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<Achievement> GetAchievementInfo(Guid achId)
    {
        var achievement = await _databaseConnection.Achievements
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == achId);

        if (achievement == null)
            throw new KeyNotFoundException($"Achievement with ID {achId} not found");

        return achievement;
    }

    public async Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId)
    {
        return await _databaseConnection.UserAchievements
            .AsNoTracking()
            .Where(q => q.UserId == userId && q.IsCompleted)
            .ToListAsync();
    }

    public async Task<List<UserAchievement>> GetUserAchievements(Guid userId)
    {
        return await _databaseConnection.UserAchievements
            .AsNoTracking()
            .Where(q => q.UserId == userId)
            .ToListAsync();
    }

    public async Task<PaginatedResult<Achievement>> GetCityAchievementsAsync(Guid cityId, int page, int pageSize)
    {
        var query = _databaseConnection.Achievements
            .AsNoTracking()
            .Where(q => q.CityId == cityId)
            .OrderBy(q => q.Title)
            .ThenBy(q => q.Id);

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Achievement>
        {
            Items = items,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<bool> DoesCategoryExistAsync(Guid categoryId)
    {
        return await _databaseConnection.AchievementCategories
            .AsNoTracking()
            .AnyAsync(q => q.Id == categoryId);
    }

    public async Task<Achievement> CreateAchievementAsync(AchievementRequest request)
    {
        var achievement = new Achievement
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        Apply(request, achievement);

        _databaseConnection.Achievements.Add(achievement);
        await _databaseConnection.SaveChangesAsync();

        return achievement;
    }

    public async Task<Achievement> UpdateAchievementAsync(Guid achId, AchievementRequest request)
    {
        var achievement = await _databaseConnection.Achievements
            .FirstOrDefaultAsync(q => q.Id == achId);

        if (achievement == null)
            throw new KeyNotFoundException($"Achievement with ID {achId} not found");

        Apply(request, achievement);

        await _databaseConnection.SaveChangesAsync();

        return achievement;
    }

    /// <summary>
    /// Переносит поля запроса в сущность. CreatedAt и Id не трогаем:
    /// дата создания при обновлении должна остаться прежней.
    /// </summary>
    private static void Apply(AchievementRequest request, Achievement achievement)
    {
        achievement.Title = request.Title.Trim();
        achievement.Description = request.Description.Trim();
        achievement.Goal = request.Goal;
        achievement.RewardScore = request.RewardScore;
        achievement.CityId = request.CityId;
        achievement.CategoryId = request.CategoryId;
        achievement.IconUrl = request.IconUrl.Trim();
        achievement.Lat = request.Lat;
        achievement.Lon = request.Lon;
        achievement.IsPartner = request.IsPartner;
    }
}
