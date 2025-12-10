using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Query.TagApiQueries.PreservedTags;
using Equinor.ProCoSys.Preservation.Query.TagApiQueries.SearchTags;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
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
        public async Task<ActionResult<List<PCSTagDto>>> SearchTagsByTagNo(
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060: Remove unused parameter")]
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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
        public async Task<ActionResult<List<PCSTagDto>>> SearchTagsByTagFunctions(
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060: Remove unused parameter")]
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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
        public async Task<ActionResult<List<PCSPreservedTagDto>>> Get(
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060: Remove unused parameter")]
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new PreservedTagsQuery(projectName));
            return this.FromResult(result);
        }
    }
}
