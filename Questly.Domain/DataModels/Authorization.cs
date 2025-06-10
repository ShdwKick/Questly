using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotChocolate;

namespace DataModels;

public class Authorization
{
    [Key] 
    [Column("id")] 
    public Guid Id { get; set; }
    
    [Required]
    [Column("f_user_id")] 
    public Guid UserId { get; set; }

    [ForeignKey("UserId")] 
    public User User { get; set; }

    [Column("c_google_token")] 
    public string? GoogleToken { get; set; }

    [Column("c_auth_token")] 
    public string AuthToken { get; set; }
    
    [GraphQLIgnore]
    [Column("c_auth_token_hash")] 
    public string AuthTokenHash { get; set; }
}