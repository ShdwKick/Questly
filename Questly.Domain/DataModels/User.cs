using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels;

public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("c_username")]
    [Required]
    public string Username { get; set; }

    [Column("c_email")]
    [Required]
    public string Email { get; set; }

    [Column("c_password_hash")]
    public string PasswordHash { get; set; }
    
    [Required]//TODO: не уверен что оно тут надо
    [Column("f_city_id")]
    public Guid CityId { get; set; }

    [ForeignKey("CityId")]
    public City? City { get; set; }

    [Column("c_created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("c_avatar_url")]
    public string? AvatarUrl { get; set; }
}