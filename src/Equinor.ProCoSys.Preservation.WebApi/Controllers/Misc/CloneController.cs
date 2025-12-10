using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.MiscCommands.Clone;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Misc
{
    [ApiController]
    [Route("Clone")]
    public class CloneController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CloneController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPut("Clone")]
        public async Task<IActionResult> Clone(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string targetPlant,
            [FromQuery]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string sourcePlant
            )
        {
            var command = new CloneCommand(sourcePlant, targetPlant);

            var result = await _mediator.Send(command);

            return this.FromResult(result);
        }
    }
}
