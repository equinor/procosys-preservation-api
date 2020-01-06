using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Responsible
{
    [ApiController]
    [Route("Responsibles")]
    public class ResponsiblesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ResponsiblesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponsibleDto>>> GetAllResponsibles()
        {
            var result = await _mediator.Send(new AllResponsiblesQuery());
            return this.FromResult(result);
        }
    }
}
