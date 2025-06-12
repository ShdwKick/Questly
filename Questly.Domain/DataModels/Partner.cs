using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels;

public class Partner
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(128)]
    [Column("c_company_name")]
    public string CompanyName { get; set; }

    [Required]
    [MaxLength(256)]
    [Column("c_address")]
    public string Address { get; set; }

    [MaxLength(256)]
    [Column("c_owner_email")]
    public string? OwnerEmail { get; set; }

    [MaxLength(11)]
    [Column("c_contact_phone")]
    public long ContactPhone { get; set; }

    [Column("c_commission_rate")]
    public float CommissionRate { get; set; }

    [Column("c_is_active")]
    public bool IsActive { get; set; }
}