using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Query.GetAllActionsCrossPlant;
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

        [HttpGet("Actions")]
        public async Task<ActionResult<List<ActionDto>>> Actions()
        {
            var result = await _mediator.Send(new GetAllActionsCrossPlantQuery());
            return this.FromResult(result);
        }
    }
}
