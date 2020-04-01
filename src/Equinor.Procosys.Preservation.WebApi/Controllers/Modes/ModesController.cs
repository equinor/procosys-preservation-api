using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode;
using Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Equinor.Procosys.Preservation.WebApi.Misc;
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
        public async Task<ActionResult<ModeDto>> GetModes(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant)
        {
            var result = await _mediator.Send(new GetAllModesQuery());
            return this.FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModeDto>> GetMode(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetModeByIdQuery(id));
            return this.FromResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddMode(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromBody] CreateModeDto dto)
        {
            var result = await _mediator.Send(new CreateModeCommand(dto.Title));
            return this.FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMode(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteModeCommand(id));
            return this.FromResult(result);
        }
    }
}
