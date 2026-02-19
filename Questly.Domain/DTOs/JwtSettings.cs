namespace DataModels.DTOs;

public class JwtSettings
{
    public string ServerKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ClockSkewMinutes { get; set; } = 1;
}