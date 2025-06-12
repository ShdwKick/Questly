using DataModels;
using Microsoft.EntityFrameworkCore;

namespace QuestlyAdmin.Services
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
                
                _logger.LogInformation("База данных инициализирована успешно!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при инициализации базы данных");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}