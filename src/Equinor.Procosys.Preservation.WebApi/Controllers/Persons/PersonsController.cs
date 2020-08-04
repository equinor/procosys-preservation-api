using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.PersonCommands.CreateSavedFilter;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Persons
{
    [ApiController]
    [Route("Persons")]
    public class PersonsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PersonsController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_CREATE)]
        [HttpPost("/SavedFilter")]
        public async Task<ActionResult<int>> CreateSavedFilter(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] CreateSavedFilterDto dto)
        {
            var result = await _mediator.Send(new CreateSavedFilterCommand(dto.Title, dto.Criteria));
            return this.FromResult(result);
        }
    }
}
