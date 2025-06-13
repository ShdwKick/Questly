using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotChocolate;

namespace DataModels;

public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(64)]
    [Column("c_username")]
    public string Username { get; set; }
    
    [Required]
    [MaxLength(156)]
    [Column("c_email")]
    public string Email { get; set; }

    [Required]
    [MaxLength(256)]
    [Column("c_password_hash")]
    [GraphQLIgnore]
    public string PasswordHash { get; set; }

    [Column("c_created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(256)]
    [Column("c_avatar_url")]
    public string? AvatarUrl { get; set; }
    
    [Column("b_blocked")]
    public bool IsBlocked { get; set; }
    
    [MaxLength(256)]
    [Column("c_block_reason")]
    public string? BlockReason { get; set; }
    
    [Column("b_admin")]
    public bool IsAdmin { get; set; }
    
    [GraphQLIgnore]
    [Column("c_salt")]
    public string Salt { get; set; }
}