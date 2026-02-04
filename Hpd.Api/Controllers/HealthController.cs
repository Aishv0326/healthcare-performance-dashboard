using Microsoft.AspNetCore.Mvc;

namespace Hpd.Api.Controllers
{
    /// <summary>
    /// Lightweight health endpoint used to verify the API is running.
    ///
    /// In production systems, health checks are commonly used by:
    /// - load balancers
    /// - container orchestrators
    /// - uptime monitors
    ///
    /// For this project, it also helps confirm HTTPS + routing works locally.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Returns a simple OK payload with a UTC timestamp.
        /// Useful for smoke tests and quick validation from Swagger or browser.
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "ok", timeUtc = DateTime.UtcNow });
        }
    }
}
