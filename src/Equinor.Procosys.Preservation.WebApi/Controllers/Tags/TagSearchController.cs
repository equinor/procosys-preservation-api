using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.TagApiQueries.PreservedTags;
using Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        /// Gets tags from ProCoSys by TagNos. DTO enriched with preservation info
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="projectName"></param>
        /// <param name="startsWithTagNo"></param>
        /// <returns>All ProCoSys tags that match the search parameters</returns>
        [Authorize(Roles = Permissions.TAG_READ)]
        [HttpGet]
        public async Task<ActionResult<List<ProcosysTagDto>>> SearchTagsByTagNo(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName,
            [FromQuery] string startsWithTagNo)
        {
            var result = await _mediator.Send(new SearchTagsByTagNoQuery(projectName, startsWithTagNo));
            return this.FromResult(result);
        }

        /// <summary>
        /// Gets tags from ProCoSys where Tag belong to a TagFunction which have any preservation requirement. DTO enriched with preservation info
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="projectName"></param>
        /// <returns>All ProCoSys tags that match the search parameters</returns>
        [Authorize(Roles = Permissions.TAG_READ)]
        [HttpGet("ByTagFunctions")]
        public async Task<ActionResult<List<ProcosysTagDto>>> SearchTagsByTagFunctions(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new SearchTagsByTagFunctionQuery(projectName));
            return this.FromResult(result);
        }

        /// <summary>
        /// Gets preservation tags from old ProCoSys. DTO enriched with preservation info from new solution
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="projectName"></param>
        /// <returns>All preserved tags in old ProCoSys</returns>
        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("Preserved")]
        public async Task<ActionResult<List<ProcosysPreservedTagDto>>> Get(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new PreservedTagsQuery(projectName));
            return this.FromResult(result);
        }
    }
}
