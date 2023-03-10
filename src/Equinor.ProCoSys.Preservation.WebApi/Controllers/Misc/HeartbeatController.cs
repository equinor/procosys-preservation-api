using System.ComponentModel.DataAnnotations;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Common.Telemetry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Equinor.ProCoSys.Preservation.WebApi.Misc;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Misc
{
    [ApiController]
    [Route("Heartbeat")]
    public class HeartbeatController : ControllerBase
    {
        private readonly ILogger<HeartbeatController> _logger;
        private readonly ITelemetryClient _telemetryClient;
        private readonly IDebugConfigValues _debugConfigValues;

        public HeartbeatController(
            ILogger<HeartbeatController> logger,
            ITelemetryClient telemetryClient,
            IDebugConfigValues debugConfigValues)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _debugConfigValues = debugConfigValues;
        }

        [AllowAnonymous]
        [HttpGet("IsAlive")]
        public IActionResult IsAlive()
        {
            var timestampString = $"{TimeService.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
            _logger.LogInformation($"The application is running at {timestampString}");
            return new JsonResult(new
            {
                IsAlive = true,
                TimeStamp = timestampString
            });
        }

        [HttpPost("LogTrace")]
        public void LogTrace()
            => _telemetryClient.TrackEvent("Heartbeat");

        [HttpPost("LogInfo")]
        public void LogInfo([Required] string info)
            => _logger.LogInformation($"Information: {info}");

        [HttpPost("LogConfigs")]
        public void LogConfigs()
            => _logger.LogInformation(_debugConfigValues.GetValues());

        [HttpPost("LogWarning")]
        public void LogWarning([Required] string warning)
            => _logger.LogWarning($"Warning: {warning}");

        [HttpPost("LogError")]
        public void LogError([Required] string error)
            => _logger.LogError($"Error: {error}");

        [HttpPost("LogException")]
        public void LogException([Required] string error)
            => _logger.LogError(new System.Exception(error), $"Exception: {error}");
    }
}
