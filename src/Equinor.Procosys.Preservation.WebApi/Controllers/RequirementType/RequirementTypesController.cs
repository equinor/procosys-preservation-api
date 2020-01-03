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
        public async Task<ActionResult<IEnumerable<RequirementTypeDto>>> GetRequirementTypes()
        {
            var requirementTypes = await _mediator.Send(new GetAllRequirementTypesQuery());
            return Ok(requirementTypes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetRequirementType([FromRoute] int id)
        {
            var dto = await _mediator.Send(new GetRequirementTypeByIdQuery(id));
            return Ok(dto);
        }
    }
}
