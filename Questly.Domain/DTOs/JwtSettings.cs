namespace DataModels.DTOs;

public class JwtSettings
{
    public string ServerKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ClockSkewMinutes { get; set; } = 1;
    
    /// <summary>
    /// Проверяет корректность настроек JWT и бросает исключение при обнаружении проблем
    /// </summary>
    /// <exception cref="InvalidOperationException">Если настройки невалидны</exception>
    public void Validate()
    {
        if (string.IsNullOrEmpty(ServerKey))
        {
            throw new InvalidOperationException("ServerKey не может быть пустым. Это критически важный параметр для создания секретного ключа.");
        }
        
        if (string.IsNullOrEmpty(Issuer))
        {
            throw new InvalidOperationException("Issuer не может быть пустым. Укажите издателя токена (обычно URL вашего сервера).");
        }
        
        if (string.IsNullOrEmpty(Audience))
        {
            throw new InvalidOperationException("Audience не может быть пустым. Укажите получателя токена (обычно URL клиентского приложения).");
        }
        
        if (ClockSkewMinutes < 0)
        {
            throw new InvalidOperationException($"ClockSkewMinutes не может быть отрицательным. Текущее значение: {ClockSkewMinutes}");
        }
    }
}