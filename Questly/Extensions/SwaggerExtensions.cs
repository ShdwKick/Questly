using System.Reflection;
using Microsoft.OpenApi;

namespace Questly.Extensions;

public static class SwaggerExtensions
{
    private const string BearerScheme = "Bearer";

    /// <summary>
    /// Описание Swagger вместе со схемой Bearer. Без SecurityDefinition
    /// в Swagger UI нет кнопки Authorize, и защищённые ручки из него
    /// невозможно вызвать — только через curl или Postman.
    /// </summary>
    public static void AddQuestlySwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Questly API",
                Version = "v1"
            });

            options.AddSecurityDefinition(BearerScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Введите access-токен. Префикс «Bearer» Swagger добавит сам.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            // Swashbuckle 10 ждёт фабрику: ссылка на схему должна знать документ,
            // в котором эта схема объявлена.
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(BearerScheme, document)] = []
            });

            // XML-комментарии контроллеров попадают в описание ручек.
            // Файла может не быть, если документация не сгенерирована, — это не ошибка.
            var xmlPath = Path.Combine(
                AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });
    }
}
