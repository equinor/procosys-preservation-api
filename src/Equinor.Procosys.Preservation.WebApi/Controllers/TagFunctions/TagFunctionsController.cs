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
        public async Task<ActionResult<IEnumerable<TagFunctionDto>>> GetAllTagFunctions([FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetAllTagFunctionsQuery(includeVoided));
            return this.FromResult(result);
        }
    }
}
