using DataModels;
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
}
