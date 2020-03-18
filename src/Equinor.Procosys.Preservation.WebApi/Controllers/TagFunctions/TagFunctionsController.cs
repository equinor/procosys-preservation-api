using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.TagFunctionAggregate;
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

        [HttpGet]
        public async Task<ActionResult<TagFunctionDto>> GetAllTagFunctions([FromQuery] string tagFunctionCode, [FromQuery] string registerCode)
        {
            var result = await _mediator.Send(new GetTagFunctionQuery(tagFunctionCode, registerCode));
            return this.FromResult(result);
        }
    }
}
