using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    [ApiController]
    [Route("Journey")]
    public class JourneyController : ControllerBase
    {
        private readonly ILogger<JourneyController> _logger;
        private readonly IMediator mediator;

        public JourneyController(ILogger<JourneyController> logger, IMediator mediator)
        {
            _logger = logger;
            this.mediator = mediator;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(1337);
        }
    }
}
