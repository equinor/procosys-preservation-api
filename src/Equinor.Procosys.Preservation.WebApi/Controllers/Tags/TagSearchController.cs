using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    /// <summary>
    /// Handles requests that deal with all ProCoSys tags, not just preservation
    /// </summary>
    [ApiController]
    [Route("Tags/Search")]
    public class TagSearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagSearchController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Gets tags from ProCoSys by TagNos, and enriches them with preservation data
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="startsWithTagNo"></param>
        /// <returns>All ProCoSys tags that match the search parameters</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProcosysTagDto>>> SearchTagsByTagNo([FromQuery] string projectName, [FromQuery] string startsWithTagNo)
        {
            var result = await _mediator.Send(new SearchTagsByTagNoQuery(projectName, startsWithTagNo));
            return this.FromResult(result);
        }

        /// <summary>
        /// Gets tags from ProCoSys by TagFunction/Register codes, and enriches them with preservation data
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="tagFunctionCodeRegisterCodePairs">Pair of TagFunctionCode and RegisterCode. Sample M|MAIN_EQUIPMENT</param>
        /// <returns>All ProCoSys tags that match the search parameters</returns>
        [HttpGet("ByTagFunctions")]
        public async Task<ActionResult<List<ProcosysTagDto>>> SearchTagsByTagFunctions([FromQuery] string projectName, [FromQuery] List<string> tagFunctionCodeRegisterCodePairs)
        {
            var result = await _mediator.Send(new SearchTagsByTagFunctionsQuery(projectName, tagFunctionCodeRegisterCodePairs));
            return this.FromResult(result);
        }
    }
}
