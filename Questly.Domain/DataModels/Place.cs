using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels;

public class Place
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("c_name")]
    public string Name { get; set; }

    [Column("c_description")]
    public string? Description { get; set; }

    [Column("c_lat")]
    public float? Lat { get; set; }

    [Column("c_lng")]
    public float? Lng { get; set; }

    [Column("f_city")]
    public Guid? CityId { get; set; }

    [ForeignKey("CityId")]
    public City City { get; set; }

    [Column("f_type_id")]
    public Guid? TypeId { get; set; }

    [ForeignKey("TypeId")]
    public PlaceType Type { get; set; }

    [Column("c_is_partner")]
    public bool IsPartner { get; set; }

    [Column("f_achievement_id")]
    public Guid? AchievementId { get; set; }

    [ForeignKey("AchievementId")]
    public Achievement Achievement { get; set; }

    [Column("f_partner_id")]
    public Guid? PartnerId { get; set; }

    [ForeignKey("PartnerId")]
    public Partner Partner { get; set; }

    [Column("c_icon_url")]
    public string IconUrl { get; set; }
}