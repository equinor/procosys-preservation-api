using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.AllAvailableTagsQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    [ApiController]
    [Route("Tags/Available")]
    public class AvailableTagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AvailableTagsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<ProcosysTagDto>>> GetAllAvailableTags([FromQuery] string projectName, [FromQuery] string startsWithTagNo)
        {
            var result = await _mediator.Send(new AllAvailableTagsQuery(projectName, startsWithTagNo));
            return this.FromResult(result);
        }
    }
}
