using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
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
        public async Task<ActionResult<IEnumerable<RequirementTypeDto>>> GetRequirementTypes([FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetAllRequirementTypesQuery(includeVoided));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementTypeDto>> GetRequirementType([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetRequirementTypeByIdQuery(id));
            return Ok(result);
        }
    }
}
