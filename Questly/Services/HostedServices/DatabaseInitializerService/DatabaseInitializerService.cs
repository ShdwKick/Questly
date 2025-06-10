using DataModels;
using Microsoft.EntityFrameworkCore;

namespace Questly.Services
{
    public class DatabaseInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializerService> _logger;

        public DatabaseInitializerService(IServiceProvider serviceProvider, ILogger<DatabaseInitializerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            _logger.LogInformation("Инициализация базы данных...");

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
                    _logger.LogInformation("Добавлен тестовый администратор");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при инициализации базы данных");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}