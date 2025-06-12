using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels;

public class BlockUser
{
    [Key]
    [Column("Id")] 
    public Guid Id { get; set; }
    
    [Required] 
    [Column("f_user_id")] 
    public Guid UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User? User { get; set; }
    
    [Column("b_block_status")] 
    public bool BlockStatus { get; set; }
    
    [MaxLength(256)] 
    [Column("c_reason")] 
    public string? Reason { get; set; }
    
    [Column("d_modif_datetime")] 
    public DateTime ModifDateTime { get; set; } = DateTime.UtcNow;
}