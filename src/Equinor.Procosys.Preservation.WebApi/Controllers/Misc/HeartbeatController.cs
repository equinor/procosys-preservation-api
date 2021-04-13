using Equinor.ProCoSys.Preservation.Domain.Time;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Misc
{
    [ApiController]
    [Route("Heartbeat")]
    public class HeartbeatController : ControllerBase
    {
        private readonly ILogger<HeartbeatController> _logger;

        public HeartbeatController(ILogger<HeartbeatController> logger) => _logger = logger;

        [AllowAnonymous]
        [HttpGet("IsAlive")]
        public IActionResult IsAlive()
        {
            var timestampString = $"{TimeService.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
            _logger.LogDebug($"The application is running at {timestampString}");
            return new JsonResult(new
            {
                IsAlive = true,
                TimeStamp = timestampString
            });
        }
    }
}
