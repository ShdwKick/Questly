using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels;

public class Leaderboard
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("f_user_id")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [Column("f_city")]
    public Guid? CityId { get; set; }

    [ForeignKey("CityId")]
    public City City { get; set; }

    [Column("c_score")]
    public int Score { get; set; }
}