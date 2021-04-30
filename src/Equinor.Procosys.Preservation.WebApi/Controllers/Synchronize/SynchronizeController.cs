using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.SyncCommands.SyncProjects;
using Equinor.ProCoSys.Preservation.Command.SyncCommands.SyncResponsibles;
using Equinor.ProCoSys.Preservation.Command.SyncCommands.SyncTagFunctions;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Synchronize
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
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant)
        {
            var result = await _mediator.Send(new SyncTagFunctionsCommand());
            return this.FromResult(result);
        }
    }
}
