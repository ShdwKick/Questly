using DataModels.Helpers;

namespace Questly.Infrastructure.Helpers;

public class ConfigurationHelper : IConfigurationHelper
{
    private readonly IConfiguration _configuration;

    public ConfigurationHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string? GetSalt() => _configuration["HASH_SALT"] ?? _configuration["HashSalt"];
    public string? GetServerKey() => _configuration["SERVER_KEY"] ?? _configuration["ServerKey"] ?? _configuration["Jwt:ServerKey"];
    public string? GetIssuer() => _configuration["ISSUER"] ?? _configuration["Issuer"] ?? _configuration["Jwt:Issuer"];
    public string? GetAudience() => _configuration["AUDIENCE"] ?? _configuration["Audience"] ?? _configuration["Jwt:Audience"];
    public string? GetRabbitHostName() => _configuration["RABBITMQ_HOSTNAME"] ?? _configuration["RabbitMQ:HostName"];
    public string? GetRabbitUserName() => _configuration["RABBITMQ_USERNAME"] ?? _configuration["RabbitMQ:UserName"];
    public string? GetRabbitPassword() => _configuration["RABBITMQ_PASSWORD"] ?? _configuration["RabbitMQ:Password"];
}