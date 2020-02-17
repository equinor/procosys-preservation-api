using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetAreas;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Areas
{
    [ApiController]
    [Route("Areas")]
    public class AreasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AreasController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<AreaDto>>> GetAreas()
        {
            var result = await _mediator.Send(new GetAreasQuery());
            return this.FromResult(result);
        }
    }
}
