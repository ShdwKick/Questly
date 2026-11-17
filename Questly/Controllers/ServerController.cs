using DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Questly.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServerController(
    DatabaseContext databaseContext,
    ILogger<ServerController> logger) : ControllerBase
{
    /// <summary>
    /// Текущее время сервера в его локальной зоне
    /// </summary>
    [HttpGet("datetime")]
    [ProducesResponseType(typeof(DateTime), StatusCodes.Status200OK)]
    public ActionResult<DateTime> GetDateTime()
    {
        return Ok(DateTime.Now);
    }

    /// <summary>
    /// Текущее время сервера в UTC
    /// </summary>
    [HttpGet("datetime/utc")]
    [ProducesResponseType(typeof(DateTime), StatusCodes.Status200OK)]
    public ActionResult<DateTime> GetUtcDateTime()
    {
        return Ok(DateTime.UtcNow);
    }

    /// <summary>
    /// Состояние сервиса и его базы данных: отвечает 503, если база недоступна
    /// или схема не применена. Пригодно для внешнего мониторинга и для ручной
    /// проверки после деплоя. Как HEALTHCHECK контейнера напрямую не годится:
    /// в образе mcr.microsoft.com/dotnet/aspnet нет ни curl, ни wget.
    /// </summary>
    /// <response code="200">Сервис готов принимать запросы</response>
    /// <response code="503">База недоступна или есть неприменённые миграции</response>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth(CancellationToken cancellationToken)
    {
        try
        {
            var canConnect = await databaseContext.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
            {
                logger.LogWarning("Healthcheck: база данных недоступна");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { status = "unhealthy", database = "unreachable" });
            }

            var pending = (await databaseContext.Database
                .GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (pending.Count != 0)
            {
                logger.LogWarning("Healthcheck: не применены миграции: {Pending}", string.Join(", ", pending));
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { status = "unhealthy", database = "migrations_pending", pendingMigrations = pending });
            }

            return Ok(new { status = "healthy", database = "ready" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Healthcheck: ошибка при проверке базы данных");
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { status = "unhealthy", database = "error" });
        }
    }
}
