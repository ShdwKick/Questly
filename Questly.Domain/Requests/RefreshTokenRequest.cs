namespace DataModels.Requests;

/// <summary>
/// DTO для обновления токена
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
}