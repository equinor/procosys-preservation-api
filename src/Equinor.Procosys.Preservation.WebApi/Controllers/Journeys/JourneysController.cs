using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep;
using Equinor.Procosys.Preservation.Query.JourneyAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "LIBRARY_PRESERVATION/READ,PRESERVATION/READ")]
        [HttpGet]
        public async Task<ActionResult<List<JourneyDto>>> GetJourneys([FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetAllJourneysQuery(includeVoided));
            return this.FromResult(result);
        }

        [Authorize(Roles = "LIBRARY_PRESERVATION/READ,PRESERVATION/READ")]
        [HttpGet("{id}")]
        public async Task<ActionResult<JourneyDto>> GetJourney([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetJourneyByIdQuery(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = "LIBRARY_PRESERVATION/CREATE")]
        [HttpPost]
        public async Task<ActionResult<int>> AddJourney([FromBody] CreateJourneyDto dto)
        {
            var result = await _mediator.Send(new CreateJourneyCommand(dto.Title));
            return this.FromResult(result);
        }

        [Authorize(Roles = "LIBRARY_PRESERVATION/CREATE")]
        [HttpPost("{id}/AddStep")]
        public async Task<ActionResult> AddStep([FromRoute] int id, [FromBody] CreateStepDto dto)
        {
            var result = await _mediator.Send(new CreateStepCommand(id, dto.Title, dto.ModeId, dto.ResponsibleId));
            return this.FromResult(result);
        }
    }
}
