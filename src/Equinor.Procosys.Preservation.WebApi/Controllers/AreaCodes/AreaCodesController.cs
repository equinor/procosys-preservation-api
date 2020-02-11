using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetAreaCodes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.AreaCodes
{
    [ApiController]
    [Route("AreaCodes")]
    public class AreaCodesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AreaCodesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<AreaCodeDto>>> GetJourneys()
        {
            var result = await _mediator.Send(new GetAreaCodesQuery());
            return this.FromResult(result);
        }
    }
}
