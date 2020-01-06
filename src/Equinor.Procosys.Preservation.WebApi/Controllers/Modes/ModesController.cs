using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode;
using Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Modes
{
    [ApiController]
    [Route("Modes")]
    public class ModesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ModesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetModes()
        {
            var result = await _mediator.Send(new GetAllModesQuery());
            return this.FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMode([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetModeByIdQuery(id));
            return this.FromResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddMode([FromBody] CreateModeDto dto)
        {
            var result = await _mediator.Send(new CreateModeCommand { Title = dto.Title });
            return this.FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMode([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteModeCommand(id));
            return this.FromResult(result);
        }
    }
}
