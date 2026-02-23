using Elastic.Clients.Elasticsearch;
using Elastic.Serilog.Sinks;
using Questly.Exceptions;
using Questly.Extensions;
using Questly.Middlewares;
using Serilog;
using DataStreamName = Elastic.Ingest.Elasticsearch.DataStreams.DataStreamName;

namespace Questly;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddEnvironmentVariables();
        
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<HttpResponseExceptionFilter>();
        });
            
        if (!string.IsNullOrEmpty(builder.Configuration["CONNECTION_STRING"]))
        {
            builder.Configuration["ConnectionStrings:Default"] = 
                builder.Configuration["CONNECTION_STRING"];
        }
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateBootstrapLogger();
        
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(
                new[] { new Uri("http://elasticsearch:9200") },
                opts =>
                {
                    opts.DataStream = new DataStreamName("logs-myapp-development");
                })
            .CreateLogger();

        builder.Host.UseSerilog();


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
        
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            Log.Information("Application started test log");
        });

        app.Run();
    }
}