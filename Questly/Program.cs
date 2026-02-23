using Questly.Extensions;
using Questly.Middlewares;

namespace Questly;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
            
        if (!string.IsNullOrEmpty(builder.Configuration["CONNECTION_STRING"]))
        {
            builder.Configuration["ConnectionStrings:Default"] = 
                builder.Configuration["CONNECTION_STRING"];
        }

        // Конфигурация и регистрация сервисов через extension-методы
        builder.Services.AddQuestlyConfiguration(builder.Configuration);
        builder.Services.AddQuestlyServices();
        builder.Services.AddQuestlyAuthentication(builder.Configuration);
            
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
            
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
        builder.WebHost.UseUrls("http://0.0.0.0:5000", "http://localhost:5000");

        var app = builder.Build();
            
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseResponseSizeLimit();
        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}