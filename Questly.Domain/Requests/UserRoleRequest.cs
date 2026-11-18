using System.ComponentModel.DataAnnotations;

namespace DataModels.Requests;

/// <summary>
/// Изменение признака администратора у пользователя.
/// </summary>
public class UserRoleRequest
{
    /// <summary>
    /// Выдать (true) или снять (false) права администратора.
    /// Поле nullable с [Required] намеренно: иначе пропущенное в теле поле
    /// молча читалось бы как false и запрос на выдачу прав снимал бы их.
    /// </summary>
    [Required]
    public bool? IsAdmin { get; set; }
}
