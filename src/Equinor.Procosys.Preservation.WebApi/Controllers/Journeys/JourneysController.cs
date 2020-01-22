using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep;
using Equinor.Procosys.Preservation.Query.JourneyAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    [ApiController]
    [Route("Journeys")]
    public class JourneysController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JourneysController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{id}")]
        public async Task<ActionResult<JourneyDto>> GetJourney([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetJourneyByIdQuery(id));
            return this.FromResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddJourney([FromBody] CreateJourneyDto dto)
        {
            var result = await _mediator.Send(new CreateJourneyCommand(dto.Title));
            return this.FromResult(result);
        }

        [HttpPost("{id}/AddStep")]
        public async Task<ActionResult> AddStep([FromRoute] int id, [FromBody] CreateStepDto dto)
        {
            var result = await _mediator.Send(new CreateStepCommand(id, dto.ModeId, dto.ResponsibleId));
            return this.FromResult(result);
        }
    }
}
