using DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Questly.Services;

/// <summary>
/// Готовит базу к работе до того, как приложение начнёт принимать запросы:
/// создаёт базу, если её нет, применяет миграции и засевает справочники.
///
/// Запускается на каждом старте контейнера, поэтому деплой на пустой том
/// (или на сервер, где базы ещё нет) поднимает схему сам, без ручных шагов.
/// Если база недоступна, инициализация повторяется с нарастающей задержкой —
/// контейнер приложения может стартовать раньше, чем Postgres примет соединения.
/// Когда попытки исчерпаны, исключение пробрасывается: хост не поднимается,
/// контейнер падает и перезапускается политикой restart. Это намеренно —
/// приложение без схемы отвечало бы 500 на каждый запрос.
/// </summary>
public class DatabaseInitializerService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseInitializerService> logger)
    : IHostedService
{
    private const int MaxAttempts = 12;
    private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan MaxDelay = TimeSpan.FromSeconds(15);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var delay = InitialDelay;

        for (var attempt = 1; attempt < MaxAttempts; attempt++)
        {
            try
            {
                await InitializeAsync(cancellationToken);
                return;
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex,
                    "Не удалось инициализировать базу данных (попытка {Attempt} из {MaxAttempts}). Повтор через {Delay} с",
                    attempt, MaxAttempts, delay.TotalSeconds);

                await Task.Delay(delay, cancellationToken);
                delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, MaxDelay.TotalSeconds));
            }
        }

        // Последняя попытка — без перехвата, чтобы исключение дошло до хоста и остановило старт.
        await InitializeAsync(cancellationToken);
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        logger.LogInformation("Инициализация базы данных...");

        var creator = dbContext.Database.GetService<IRelationalDatabaseCreator>();
        var databaseExists = await creator.ExistsAsync(cancellationToken);

        if (databaseExists)
        {
            logger.LogInformation("База данных найдена");
        }
        else
        {
            logger.LogInformation("База данных отсутствует — будет создана при применении миграций");
        }

        var pending = databaseExists
            ? (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList()
            : dbContext.Database.GetMigrations().ToList();

        if (pending.Count != 0)
        {
            logger.LogInformation("Применяются миграции ({Count}): {Migrations}",
                pending.Count, string.Join(", ", pending));
        }

        // MigrateAsync сам создаёт базу, если её нет, а затем накатывает все миграции.
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (!databaseExists)
        {
            logger.LogInformation("База данных создана, схема применена");
        }
        else if (pending.Count != 0)
        {
            logger.LogInformation("Миграции применены");
        }
        else
        {
            logger.LogInformation("Схема базы данных актуальна");
        }

        await SeedAsync(dbContext, cancellationToken);
        await GrantAdminsAsync(dbContext, configuration, cancellationToken);
    }

    private async Task SeedAsync(DatabaseContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Cities.AnyAsync(cancellationToken))
            return;

        dbContext.Cities.Add(new City
        {
            Id = Guid.NewGuid(),
            Name = "Чебоксары",
            Description = "Столица мира",
            Lat = 56.1322,
            Lon = 47.2519,
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Добавлен стартовый город");
    }

    /// <summary>
    /// Выдаёт права администратора пользователям, перечисленным в ADMIN_USERNAMES
    /// (имена через запятую). Это способ получить первого администратора:
    /// выдавать права по API может только администратор, а в чистой базе его нет.
    ///
    /// Права только выдаются, никогда не снимаются: удаление имени из переменной
    /// не разжалует пользователя. Снять права можно через
    /// PUT /api/users/{userId}/admin. Так сделано, чтобы опечатка в переменной
    /// окружения не отобрала доступ у действующего администратора.
    /// </summary>
    private async Task GrantAdminsAsync(
        DatabaseContext dbContext,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var raw = configuration["ADMIN_USERNAMES"];
        if (string.IsNullOrWhiteSpace(raw))
            return;

        var usernames = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (usernames.Count == 0)
            return;

        var granted = 0;

        foreach (var username in usernames)
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

            if (user == null)
            {
                logger.LogWarning(
                    "Пользователь {Username} из ADMIN_USERNAMES не найден — права не выданы. " +
                    "Зарегистрируйте его и перезапустите приложение",
                    username);
                continue;
            }

            if (user.IsAdmin)
                continue;

            user.IsAdmin = true;
            granted++;

            logger.LogInformation("Пользователю {Username} выданы права администратора", username);
        }

        if (granted > 0)
            await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
