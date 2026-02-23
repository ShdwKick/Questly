namespace DataModels.Helpers;

public interface IConfigurationHelper
{
    string? GetSalt();
    string? GetServerKey();
    string? GetIssuer();
    string? GetAudience();
    string? GetRabbitHostName();
    string? GetRabbitUserName();
    string? GetRabbitPassword();
}