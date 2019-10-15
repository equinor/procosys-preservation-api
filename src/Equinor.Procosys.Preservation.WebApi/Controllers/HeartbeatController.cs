using Equinor.Procosys.Preservation.Command;
using Microsoft.AspNetCore.Mvc;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    [ApiController]
    [Route("Heartbeat")]
    public class HeartbeatController : ControllerBase
    {
        private readonly ITimeService timeService;

        public HeartbeatController(ITimeService timeService)
        {
            this.timeService = timeService;
        }

        [HttpGet]
        public IActionResult IsAlive()
        {
            return new JsonResult(new
            {
                IsAlive = true,
                TimeStamp = $"{timeService.GetCurrentTimeUTC().ToString("yyyy-MM-dd HH:mm:ss")} UTC"
            });
        }
    }
}
