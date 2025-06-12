namespace DataModels.DTOs;

public class AchievementDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Goal { get; set; } = 1;
    public int RewardScore { get; set; }
    public Guid? CityId { get; set; }
    public City City { get; set; }
    public string IconUrl { get; set; }
    public float? Lat { get; set; }
    public float? Lon { get; set; }
    public Guid? CategoryId { get; set; }
    public AchievementCategory Category { get; set; }
    public bool IsPartner { get; set; }
}