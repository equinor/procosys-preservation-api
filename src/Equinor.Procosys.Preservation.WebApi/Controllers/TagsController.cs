using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands;
using Equinor.Procosys.Preservation.Query.TagAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    [ApiController]
    [Route("Tags")]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
        {
            var tags = await _mediator.Send(new AllTagsQuery());
            return Ok(tags);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagDto dto)
        {
            var tagId = await _mediator.Send(new CreateTagCommand(dto.TagNo, dto.ProjectNo, dto.JourneyId, dto.StepId));
            return Ok(tagId);
        }

        [HttpPost("{id}/SetStep")]
        public async Task<IActionResult> SetStep([FromRoute] int id, [FromBody] SetStepDto dto)
        {
            await _mediator.Send(new SetStepCommand(id, dto.JourneyId, dto.StepId));
            return NoContent();
        }
    }
}
