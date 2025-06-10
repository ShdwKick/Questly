using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels;

public class AchievementCategory
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("c_name")]
    public string Name { get; set; }

    [Column("c_description")]
    public string? Description { get; set; }
}