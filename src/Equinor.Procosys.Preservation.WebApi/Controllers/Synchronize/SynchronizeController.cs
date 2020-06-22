using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncProjects;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncResponsibles;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncTagFunctions;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Synchronize
{
    [ApiController]
    [Route("Synchronize")]
    public class SynchronizeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SynchronizeController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_CREATE)]
        [HttpPut("Projects")]
        public async Task<ActionResult> Projects(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant)
        {
            var result = await _mediator.Send(new SyncProjectsCommand());
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("Responsibles")]
        public async Task<ActionResult> Responsibles(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant)
        {
            var result = await _mediator.Send(new SyncResponsiblesCommand());
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("TagFunctions")]
        public async Task<ActionResult> TagFunctions(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant)
        {
            var result = await _mediator.Send(new SyncTagFunctionsCommand());
            return this.FromResult(result);
        }
    }
}
