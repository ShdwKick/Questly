using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels;

public class UserAchievement
{
    [Key]
    [Column("id")]
    public Guid Id { get; init; }

    [Required]
    [Column("f_user_id")]
    public Guid UserId { get; init; }

    [ForeignKey("UserId")]
    public User User { get; init; }

    [Required]
    [Column("f_achievement_id")]
    public Guid AchievementId { get; set; }

    [ForeignKey("AchievementId")]
    public Achievement Achievement { get; init; }

    [Column("c_progress")]
    public int Progress { get; set; } = 0;
    
    [Column("b_is_completed")]
    public bool IsCompleted { get; set; } = false;

    [Column("c_earned_at")]
    public DateTime EarnedAt { get; init; } = DateTime.UtcNow;
}