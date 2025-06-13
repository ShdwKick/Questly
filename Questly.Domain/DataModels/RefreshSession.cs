using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels;

public record TokenPair(string AccessToken, string? RefreshToken);

public class RefreshSession
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("f_user_id")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [Required]
    [MaxLength(512)]
    [Column("c_refresh_token_hash")]
    public string RefreshTokenHash { get; set; }

    [MaxLength(256)]
    [Column("c_user_agent")]
    public string? UserAgent { get; set; }

    [MaxLength(64)]
    [Column("c_ip_address")]
    public string? IpAddress { get; set; }

    [Column("d_created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("d_expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("d_revoked_at")]
    public DateTime? RevokedAt { get; set; }

    [NotMapped]
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
}
