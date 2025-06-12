using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels;

public class Achievement
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(64)]
    [Column("c_title")]
    public string Title { get; set; }

    [Required]
    [MaxLength(256)]
    [Column("c_description")]
    public string Description { get; set; }

    [Column("c_goal")]
    public int Goal { get; set; } = 1;

    [Column("c_reward_score")]
    public int RewardScore { get; set; }

    [Column("f_city")]
    public Guid? CityId { get; set; }
    
    [ForeignKey("CityId")]
    public City City { get; set; }

    [MaxLength(256)]
    [Column("c_icon_url")]
    public string IconUrl { get; set; }

    [Column("c_lat")]
    public float? Lat { get; set; }

    [Column("c_lon")]
    public float? Lon { get; set; }

    [Column("f_category")]
    public Guid? CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public AchievementCategory Category { get; set; }

    [Column("c_is_partner")]
    public bool IsPartner { get; set; }

    [Column("c_created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}