using System.ComponentModel.DataAnnotations;

namespace DataModels.Requests;

/// <summary>
/// Данные города для создания или обновления.
/// Идентификатор присваивает сервер, в теле запроса он не принимается —
/// иначе клиент мог бы подменить запись, обновляя чужой город.
/// </summary>
public class CityRequest
{
    /// <summary>
    /// Название города. Должно быть уникальным без учёта регистра.
    /// </summary>
    [Required]
    [MaxLength(64)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание города.
    /// </summary>
    [MaxLength(256)]
    public string? Description { get; set; }

    /// <summary>
    /// Широта центра города.
    /// </summary>
    [Range(-90d, 90d)]
    public double? Lat { get; set; }

    /// <summary>
    /// Долгота центра города.
    /// </summary>
    [Range(-180d, 180d)]
    public double? Lon { get; set; }
}
