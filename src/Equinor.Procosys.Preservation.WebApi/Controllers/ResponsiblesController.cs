using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    [ApiController]
    [Route("Responsibles")]
    public class ResponsiblesController : ControllerBase
    {
        private readonly ILogger<JourneysController> _logger;
        private readonly IMediator mediator;

        public ResponsiblesController(ILogger<JourneysController> logger, IMediator mediator)
        {
            _logger = logger;
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponsibleDto>>> GetAllResponsibles()
        {
            List<ResponsibleDto> res = await mediator.Send(new AllResponsiblesQuery());
            return Ok(res);
        }
    }
}
