using System.Text;
using DataModels.DTOs;
using DataModels.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Questly.Helpers;
using Questly.Infrastructure.Helpers;
using Questly.Mutations;
using Questly.Queries;
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

        services.AddScoped<Mutation>();
        services.AddScoped<Query>();
        services.AddScoped<UserQueries>();
        services.AddScoped<AchievementQuery>();
        services.AddScoped<CityQuery>();
        services.AddScoped<UserMutations>();

        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddHostedService<DatabaseInitializerService>();
    }

    public static void AddQuestlyAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = new JwtSettings();
        configuration.GetSection("Jwt").Bind(jwt);
    
        // Если значения не найдены в секции Jwt, ищем в корне конфигурации
        if (string.IsNullOrEmpty(jwt.ServerKey))
            jwt.ServerKey = configuration["SERVER_KEY"] ?? configuration["ServerKey"];
        if (string.IsNullOrEmpty(jwt.Issuer))
            jwt.Issuer = configuration["ISSUER"] ?? configuration["Issuer"];
        if (string.IsNullOrEmpty(jwt.Audience))
            jwt.Audience = configuration["AUDIENCE"] ?? configuration["Audience"];

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

    [Obsolete("GraphQL был заменен на REST API. Этот метод больше не используется.")]
    public static IServiceCollection AddQuestlyGraphQl(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .ModifyRequestOptions(o => o.ExecutionTimeout = TimeSpan.FromSeconds(60))
            .AddQueryType<Query>()
            .AddTypeExtension<UserQueries>()
            .AddTypeExtension<AchievementQuery>()
            .AddTypeExtension<CityQuery>()
            .AddMutationType<Mutation>()
            .AddTypeExtension<UserMutations>()
            .AddInMemorySubscriptions()
            .AddAuthorization()
            .AddFiltering();

        return services;
    }
}