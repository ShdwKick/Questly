using Microsoft.AspNetCore.Mvc;

namespace Questly.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServerController(
    ILogger<AuthController> logger) : ControllerBase
{
    [HttpGet("datetime")]
    public ActionResult<DateTime> GetDateTime()
    {
        logger.LogError("TestError");
        return Ok(DateTime.Now);
    }

    [HttpGet("datetime/utc")]
    public ActionResult<DateTime> GetUtcDateTime()
    {
        return Ok(DateTime.UtcNow);
    }
}