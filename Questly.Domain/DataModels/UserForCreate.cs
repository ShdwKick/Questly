using System.ComponentModel.DataAnnotations;

namespace DataModels;

public class UserForCreate
{
    [MaxLength(64)]
    public string Username { get; set; }
    
    [MaxLength(128)]
    public string? Email { get; set; }
    
    [MaxLength(64)]
    public string Password { get; set; }
}