using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public async Task<IActionResult> Get()
        {
            await mediator.Send(new Tada());
            return Ok(1337);
        }
    }
}
