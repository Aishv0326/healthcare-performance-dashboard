using Microsoft.AspNetCore.Mvc;

namespace Hpd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "ok", timeUtc = DateTime.UtcNow });
        }
    }
}
