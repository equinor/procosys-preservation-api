using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands;
using Equinor.Procosys.Preservation.Query.JourneyAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    [ApiController]
    [Route("Journeys")]
    public class JourneysController : ControllerBase
    {
        private readonly ILogger<JourneysController> _logger;
        private readonly IMediator _mediator;

        public JourneysController(ILogger<JourneysController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetJourney([FromQuery] int id)
        {
            JourneyDto dto = await _mediator.Send(new GetJourneyByIdQuery(id));
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> AddJourney([FromBody] CreateJourneyDto dto)
        {
            int id = await _mediator.Send(new CreateJourneyCommand(dto.Title));
            return CreatedAtAction(nameof(JourneysController.GetJourney), id);
        }

        [HttpPost("{id}/AddStep")]
        public async Task<IActionResult> AddStep([FromRoute] int id, [FromBody] CreateStepDto dto)
        {
            await _mediator.Send(new CreateStepCommand(id, dto.ModeId, dto.TagId, dto.ResponsibleId));
            return NoContent();
        }
    }
}
