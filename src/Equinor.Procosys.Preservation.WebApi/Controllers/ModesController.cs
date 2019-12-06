using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands;
using Equinor.Procosys.Preservation.Query;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    [ApiController]
    [Route("Modes")]
    public class ModesController : ControllerBase
    {
        private readonly ILogger<ModesController> _logger;
        private readonly IMediator _mediator;

        public ModesController(ILogger<ModesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModeDto>>> GetModes()
        {
            var modes = await _mediator.Send(new GetAllModesQuery());
            return Ok(modes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetMode([FromRoute] int id)
        {
            var dto = await _mediator.Send(new GetModeByIdQuery(id));
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> AddMode([FromBody] CreateModeDto dto)
        {
            var id = await _mediator.Send(new CreateModeCommand { Title = dto.Title });
            return CreatedAtAction(nameof(GetMode), new { id }, new { id });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMode([FromRoute] int id)
        {
            await _mediator.Send(new DeleteModeCommand(id));
            return NoContent();
        }
    }
}
