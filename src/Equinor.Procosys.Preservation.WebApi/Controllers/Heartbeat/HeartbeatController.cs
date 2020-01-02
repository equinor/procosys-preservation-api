using Equinor.Procosys.Preservation.Command;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Heartbeat
{
    [ApiController]
    [Route("Heartbeat")]
    public class HeartbeatController : ControllerBase
    {
        private readonly ITimeService _timeService;
        private readonly ILogger<HeartbeatController> _logger;

        public HeartbeatController(ITimeService timeService, ILogger<HeartbeatController> logger)
        {
            _timeService = timeService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("IsAlive")]
        public IActionResult IsAlive()
        {
            var timestampString = $"{_timeService.GetCurrentTimeUTC().ToString("yyyy-MM-dd HH:mm:ss")} UTC";
            _logger.LogDebug($"The application is running at {timestampString}");
            return new JsonResult(new
            {
                IsAlive = true,
                TimeStamp = timestampString
            });
        }
    }
}
