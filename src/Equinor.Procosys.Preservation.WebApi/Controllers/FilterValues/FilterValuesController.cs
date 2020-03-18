using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetUniqueTagRequirementTypes;
using Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibleCodes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.FilterValues
{
    [ApiController]
    [Route("FilterValues")]
    public class FilterValuesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FilterValuesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("RequirementTypes")]
        public async Task<ActionResult<List<RequirementTypeDto>>> GetRequirementType([FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagRequirementTypesQuery(projectName));
            return this.FromResult(result);
        }

        [HttpGet("Responsibles")]
        public async Task<ActionResult<List<ResponsibleDto>>> GetJourneys([FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagResponsibleCodesQuery(projectName));
            return this.FromResult(result);
        }

        //[HttpGet("Journeys")]
        //public async Task<ActionResult<List<JourneyDto>>> GetJourneys([FromQuery] string projectName)
        //{
        //    var result = await _mediator.Send(new GetUniqueTagJourneysQuery(projectName));
        //    return this.FromResult(result);
        //}
    }
}
