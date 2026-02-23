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

namespace Questly.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuestlyConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            
            var conn = configuration.GetConnectionString("Default");
            if (!string.IsNullOrEmpty(conn))
            {
                services.AddDbContextPool<DataModels.DatabaseContext>(opt =>
                    opt.UseNpgsql(conn));
            }

            return services;
        }

        public static IServiceCollection AddQuestlyServices(this IServiceCollection services)
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

            return services;
        }

        public static IServiceCollection AddQuestlyAuthentication(this IServiceCollection services, IConfigurationHelper configHelper)
        {
            var jwt = new JwtSettings
            {
                ServerKey = configHelper.GetServerKey(),
                Audience = configHelper.GetAudience(),
                Issuer = configHelper.GetIssuer(),
            };
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

            return services;
        }

        [Obsolete("GraphQL был заменен на REST API. Этот метод больше не используется.")]
        public static IServiceCollection AddQuestlyGraphQL(this IServiceCollection services)
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
}
