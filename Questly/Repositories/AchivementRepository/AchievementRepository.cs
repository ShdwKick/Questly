using DataModels;
using Microsoft.EntityFrameworkCore;
using Questly.DataBase;

namespace Questly.Repositories
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly DatabaseContext _databaseConnection;

        public AchievementRepository(DatabaseContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<Achievement> GetAchievementInfo(Guid achId)
        {
            var achievement = await _databaseConnection.Achievements.FirstOrDefaultAsync(q => q.Id == achId);
            if (achievement == null)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage($"Achievement with id {achId} not found")
                        .SetCode("ACHIEVEMENT_NOT_FOUND")
                        .Build());

            return achievement;
        }

        public async Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId)
        {
            return await _databaseConnection.UserAchievements.Where(q => q.UserId == userId || q.IsCompleted).ToListAsync();
        }

        public async Task<List<UserAchievement>> GetUserAchievements(Guid userId)
        {
            return await _databaseConnection.UserAchievements.Where(q => q.UserId == userId).ToListAsync();
        }
        
        public IQueryable<Achievement> GetCityAchievements(Guid cityId)
        {
            return _databaseConnection.Achievements.Where(q => q.CategoryId == cityId).AsQueryable();
        }
    }
}