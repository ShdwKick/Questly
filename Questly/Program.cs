using Questly.Extensions;
using Questly.Middlewares;

namespace Questly
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Конфигурация и регистрация сервисов через extension-методы
            builder.Services.AddQuestlyConfiguration(builder.Configuration);
            builder.Services.AddQuestlyServices();
            builder.Services.AddQuestlyAuthentication(builder.Configuration);
            
            // Добавление CORS с правильным синтаксисом
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // Дополнительные настройки хоста
            builder.WebHost.UseUrls("http://0.0.0.0:5000");

            var app = builder.Build();

            app.UseResponseSizeLimit();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}