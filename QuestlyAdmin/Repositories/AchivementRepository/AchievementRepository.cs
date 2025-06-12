using DataModels;
using DataModels.DTOs;
using Microsoft.EntityFrameworkCore;
using QuestlyAdmin.DataBase;

namespace QuestlyAdmin.Repositories
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
                throw new Exception($"Achievement with id {achId} not found");

            return achievement;
        }

        public async Task<List<UserAchievement>> GetUserCompletedAchievements(Guid userId)
        {
            return await _databaseConnection.UserAchievements.Where(q => q.UserId == userId && q.IsCompleted).ToListAsync();
        }

        public async Task<List<UserAchievement>> GetUserAchievements(Guid userId)
        {
            return await _databaseConnection.UserAchievements.Where(q => q.UserId == userId).ToListAsync();
        }

        public async Task<bool> CreateAchievements(List<AchievementDTO> achievements)
        {
            var newAchievements = new List<Achievement>();
            foreach (var dto in achievements)
            {
                if(await DoesAchievementExist(dto.Title))
                    continue;
                
                newAchievements.Add(new Achievement
                {
                    Id = Guid.NewGuid(),
                    Title = dto.Title,
                    Description = dto.Description,
                    IconUrl = dto.IconUrl,
                    Category = dto.Category,
                    CategoryId = dto.CategoryId,
                    City = dto.City,
                    CityId = dto.CityId,
                    CreatedAt = DateTime.UtcNow,
                    Lat = dto.Lat,
                    Lon = dto.Lon,
                    Goal = dto.Goal,
                    RewardScore = dto.RewardScore,
                    IsPartner = dto.IsPartner
                });
            }
            if(newAchievements.Count == 0)
                return false;
            
            await _databaseConnection.Achievements.AddRangeAsync(newAchievements);
            var affectedRows = await _databaseConnection.SaveChangesAsync();

            return affectedRows > 0;
        }

        public async Task<bool> UpdateAchievement(Achievement achievement)
        {
            var achivment = await _databaseConnection.Achievements.FindAsync(achievement.Id);

            achivment = achievement;
            _databaseConnection.Achievements.Update(achivment);
            
            var affectedRows = await _databaseConnection.SaveChangesAsync();

            return affectedRows > 0;
        }

        public async Task<bool> RemoveAchievements(List<Guid> achievementsId)
        {
            var achievements = await _databaseConnection.Achievements.Where(q => achievementsId.Contains(q.Id)).ToListAsync();
            if (achievements.Count == 0)
                return false;
            
            _databaseConnection.Achievements.RemoveRange(achievements);
            
            var affectedRows = await _databaseConnection.SaveChangesAsync();

            return affectedRows > 0;
        }

        public async Task<bool> DoesAchievementExist(Guid achievementId)
        {
            return await _databaseConnection.Achievements.AnyAsync(q => q.Id == achievementId);
        }

        public async Task<bool> DoesAchievementExist(string achievementName)
        {
            return await _databaseConnection.Achievements.AnyAsync(q => q.Title == achievementName);
        }
    }
}