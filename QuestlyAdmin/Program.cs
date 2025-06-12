using System.Text;
using DataModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using QuestlyAdmin.Helpers;
using QuestlyAdmin.Queries;
using QuestlyAdmin.Repositories;
using QuestlyAdmin.Services;
using QuestlyAdmin.DataBase;
using QuestlyAdmin.Mutations;

namespace QuestlyAdmin
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Настройка авторизации
            builder.Services.AddAuthorization();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMemoryCache();
            //builder.Services.AddWebSockets(options => { options.KeepAliveInterval = TimeSpan.FromSeconds(120); });

            builder.Services.AddHostedService<DatabaseInitializerService>();

            //builder.Services.AddScoped<Subsription>();
            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                var connectionString = builder.Configuration["CONNECTION_STRING"];
                options.UseNpgsql(connectionString);
            });

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
            builder.Services.AddScoped<IAchievementService, AchievementService>();
            builder.Services.AddScoped<ICityService, CityService>();
            
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();
            builder.Services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            
            //builder.Services.AddSingleton<IRabbitService, RabbitService>();
            //builder.Services.AddHostedService<RabbitHostedService>();

            builder.Services.AddGraphQLServer()
                .ModifyRequestOptions(options =>
                {
                    //ограничение на максимальное время запроса
                    options.ExecutionTimeout = TimeSpan.FromSeconds(60);
                })
                .AddQueryType<Query>()
                .AddTypeExtension<UserQueries>()
                .AddTypeExtension<AchievementQuery>()
                .AddTypeExtension<CityQuery>()
                
                .AddMutationType<Mutation>()
                .AddTypeExtension<UserMutations>()
                .AddTypeExtension<AchievementMutations>()
                .AddTypeExtension<CitiesMutations>()
                
                //.AddSubscriptionType<Subsription>()
                .AddInMemorySubscriptions()
                .AddAuthorization();

            //builder.Services.AddControllers();

            var key = ConfigurationHelper.GetServerKey();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // Настройка аутентификации JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = securityKey,
                        ValidateIssuer = true,
                        ValidIssuer = ConfigurationHelper.GetIssuer(),
                        ValidateAudience = true,
                        ValidAudience = ConfigurationHelper.GetAudience(),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = _ => Task.FromResult("AUTH_FAILED_PROBLEM")
                    };
                });
            builder.WebHost.UseUrls("http://0.0.0.0:5000");
            
             var app = builder.Build();

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWebSockets();

            app.MapGraphQL();
            //app.MapControllers();
            
            app.UseCors("AllowAll");


            //set response max size limits 
            app.Use(async (context, next) =>
            {
                var originalBody = context.Response.Body;
                await using var limitedStream = new MemoryStream();
                context.Response.Body = limitedStream;

                await next();

                if (limitedStream.Length > (1024 * 1024) * 3) // 3MB
                {
                    context.Response.StatusCode = StatusCodes.Status413RequestEntityTooLarge;
                    await context.Response.WriteAsync("Response too large");
                }
                else
                {
                    limitedStream.Seek(0, SeekOrigin.Begin);
                    await limitedStream.CopyToAsync(originalBody);
                }

                context.Response.Body = originalBody;
            });

            BaseHelper.InitializeServiceProvider(app.Services);

            app.Run();
        }
    }
}