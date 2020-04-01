using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementType
{
    [ApiController]
    [Route("RequirementTypes")]
    public class RequirementTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequirementTypesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequirementTypeDto>>> GetRequirementTypes(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetAllRequirementTypesQuery(includeVoided));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementTypeDto>> GetRequirementType(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetRequirementTypeByIdQuery(id));
            return Ok(result);
        }
    }
}
