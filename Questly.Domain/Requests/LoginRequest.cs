namespace DataModels.Requests;

/// <summary>
/// DTO для аутентификации
/// </summary>
public class LoginRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
}