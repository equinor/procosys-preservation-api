using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    [ApiController]
    [Route("Journeys")]
    public class JourneysController : ControllerBase
    {
        private readonly ILogger<JourneysController> _logger;
        private readonly IMediator mediator;

        public JourneysController(ILogger<JourneysController> logger, IMediator mediator)
        {
            _logger = logger;
            this.mediator = mediator;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new OkObjectResult(new System.Collections.Generic.List<string>
            {
                "Transport",
                "Hookup",
                "Commissioning",
                "Operation"
            });
        }
    }
}
