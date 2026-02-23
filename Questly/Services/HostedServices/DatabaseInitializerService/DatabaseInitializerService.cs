using DataModels;
using Microsoft.EntityFrameworkCore;

namespace Questly.Services;

public class DatabaseInitializerService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseInitializerService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        logger.LogInformation("Инициализация базы данных...");

        try
        {
            //await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);
                
            if (!dbContext.Cities.Any())
            {
                dbContext.Cities.Add(new City
                {
                    Id = Guid.NewGuid(),
                    Name = "Чебоксары",
                    Description = "Столица мира",
                    Lat = 56.1322,
                    Lon = 47.2519,
                });

                await dbContext.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Добавлен тестовый город");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при инициализации базы данных");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}