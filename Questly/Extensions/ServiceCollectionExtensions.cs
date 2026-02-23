using System.Text;
using DataModels.DTOs;
using DataModels.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Questly.Helpers;
using Questly.Infrastructure.Helpers;
using Questly.Repositories;
using Questly.Services;

namespace Questly.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddQuestlyConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            
        var conn = configuration.GetConnectionString("Default");
        if (!string.IsNullOrEmpty(conn))
        {
            services.AddDbContextPool<DataModels.DatabaseContext>(opt =>
                opt.UseNpgsql(conn));
        }
    }

    public static void AddQuestlyServices(this IServiceCollection services)
    {
        services.AddSingleton<IConfigurationHelper, ConfigurationHelper>();
        services.AddSingleton<ITokenHelper, TokenHelper>();
        services.AddSingleton<IHeaderHelper, HeaderHelper>();
            
        // Repositories / Services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<ICityService, CityService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();

        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddHostedService<DatabaseInitializerService>();
    }

    public static void AddQuestlyAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = new JwtSettings();
        configuration.GetSection("Jwt").Bind(jwt);
    
        jwt.ServerKey = configuration["SERVER_KEY"] ?? 
                        configuration["Jwt:ServerKey"] ?? 
                        configuration["ServerKey"] ?? 
                        jwt.ServerKey;
    
        jwt.Issuer = configuration["ISSUER"] ?? 
                     configuration["Jwt:Issuer"] ?? 
                     configuration["Issuer"] ?? 
                     jwt.Issuer;
    
        jwt.Audience = configuration["AUDIENCE"] ?? 
                       configuration["Jwt:Audience"] ?? 
                       configuration["Audience"] ?? 
                       jwt.Audience;
        
        jwt.Validate();

        var keyBytes = Encoding.UTF8.GetBytes(jwt.ServerKey!);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var tokenValidation = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(jwt.ClockSkewMinutes)
        };

        services.AddSingleton(tokenValidation);
        services.AddSingleton(securityKey);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidation;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = _ => Task.CompletedTask
                };
            });
    }
}