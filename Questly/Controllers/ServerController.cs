using Microsoft.AspNetCore.Mvc;

namespace Questly.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerController : ControllerBase
    {
        [HttpGet("datetime")]
        public ActionResult<DateTime> GetDateTime()
        {
            return Ok(DateTime.Now);
        }

        [HttpGet("datetime/utc")]
        public ActionResult<DateTime> GetUtcDateTime()
        {
            return Ok(DateTime.UtcNow);
        }
    }
}

