using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.AllAvailableTagsQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    /// <summary>
    /// Handles requests that deal with all ProCoSys tags, not just preservation
    /// </summary>
    [ApiController]
    [Route("Tags/Available")]
    public class AvailableTagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AvailableTagsController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Gets all tags from ProCoSys and encriches them with preservation data
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="startsWithTagNo"></param>
        /// <returns>All ProCoSys tags that match the search parameters</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProcosysTagDto>>> GetAllAvailableTags([FromQuery] string projectName, [FromQuery] string startsWithTagNo)
        {
            var result = await _mediator.Send(new GetAllAvailableTagsQuery(projectName, startsWithTagNo));
            return this.FromResult(result);
        }
    }
}
