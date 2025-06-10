using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels;

public class Partner
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("c_company_name")]
    public string CompanyName { get; set; }

    [Required]
    [Column("c_address")]
    public string Address { get; set; }

    [Column("c_owner_email")]
    public string? OwnerEmail { get; set; }

    [Column("c_contact_phone")]
    public long ContactPhone { get; set; }

    [Column("c_commission_rate")]
    public float CommissionRate { get; set; }

    [Column("c_is_active")]
    public bool IsActive { get; set; }
}