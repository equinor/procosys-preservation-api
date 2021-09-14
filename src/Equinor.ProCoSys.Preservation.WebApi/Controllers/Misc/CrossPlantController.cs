using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Query.GetActionsCrossPlant;
using Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Misc
{
    [Authorize]
    [ApiController]
    [Route("CrossPlant")]
    public class CrossPlantController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CrossPlantController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = AppRoles.CROSSPLANT)]
        [HttpGet("Actions")]
        public async Task<ActionResult<List<ActionDto>>> Actions([FromQuery] int maxActions = 0)
        {
            var result = await _mediator.Send(new GetActionsCrossPlantQuery(maxActions));
            return this.FromResult(result);
        }

        [Authorize(Roles = AppRoles.CROSSPLANT)]
        [HttpGet("Tags")]
        public async Task<ActionResult<List<TagDto>>> Tags([FromQuery] int maxTags = 0)
        {
            var result = await _mediator.Send(new GetTagsCrossPlantQuery(maxTags));
            return this.FromResult(result);
        }
    }
}
