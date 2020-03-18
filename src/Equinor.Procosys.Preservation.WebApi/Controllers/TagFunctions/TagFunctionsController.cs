using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetTagFunctionDetails;
using Equinor.Procosys.Preservation.Query.GetTagFunctionsHavingRequirement;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.TagFunctions
{
    [ApiController]
    [Route("TagFunctions")]
    public class TagFunctionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagFunctionsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("HavingRequirement")]
        public async Task<ActionResult<IEnumerable<TagFunctionDto>>> GetTagFunctionsHavingRequirement()
        {
            var result = await _mediator.Send(new GetTagFunctionsHavingRequirementQuery());
            return this.FromResult(result);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<TagFunctionDetailsDto>> GetTagFunctionDetails([FromRoute] string code, [FromQuery] string registerCode)
        {
            var result = await _mediator.Send(new GetTagFunctionQuery(code, registerCode));
            return this.FromResult(result);
        }
    }
}
