using System.ComponentModel.DataAnnotations;

namespace DataModels.Requests;

/// <summary>
/// Данные достижения для создания или обновления.
/// Идентификатор и дату создания присваивает сервер.
/// </summary>
public class AchievementRequest
{
    /// <summary>
    /// Название достижения.
    /// </summary>
    [Required]
    [MaxLength(64)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание достижения.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Сколько раз нужно выполнить условие, чтобы достижение закрылось.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Goal { get; set; } = 1;

    /// <summary>
    /// Награда в очках за закрытие достижения.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int RewardScore { get; set; }

    /// <summary>
    /// Город, к которому привязано достижение. Если указан — должен существовать.
    /// </summary>
    public Guid? CityId { get; set; }

    /// <summary>
    /// Категория достижения. Если указана — должна существовать.
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Ссылка на иконку. Колонка в базе не допускает NULL, поэтому поле обязательное.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string IconUrl { get; set; } = string.Empty;

    /// <summary>
    /// Широта точки достижения.
    /// </summary>
    [Range(-90d, 90d)]
    public float? Lat { get; set; }

    /// <summary>
    /// Долгота точки достижения.
    /// </summary>
    [Range(-180d, 180d)]
    public float? Lon { get; set; }

    /// <summary>
    /// Достижение от партнёра.
    /// </summary>
    public bool IsPartner { get; set; }
}
