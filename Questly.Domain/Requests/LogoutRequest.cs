namespace DataModels.Requests;

/// <summary>
/// DTO для выхода из системы
/// </summary>
public class LogoutRequest
{
    public string RefreshToken { get; set; }
}