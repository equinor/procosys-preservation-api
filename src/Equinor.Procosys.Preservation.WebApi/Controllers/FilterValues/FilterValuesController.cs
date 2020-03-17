using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetUniqueTagRequirementTypes;
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

        // Henning: I've added code for 2 more endpoints to let you see how I think .... one endpoint pr. filter type. There will be for Responsible, Area, Mode, Discipline, TagFunction too
        // Implementation inside each GetUniqueTagXXXXXQuery will be as GetUniqueTagRequirementTypesQuery: pick unique used values for all tags in project to give user filter value to choose from
        //[HttpGet("Steps")]
        //public async Task<ActionResult<List<StepDto>>> GetSteps([FromQuery] string projectName)
        //{
        //    var result = await _mediator.Send(new GetUniqueTagStepsQuery(projectName));
        //    return this.FromResult(result);
        //}

        //[HttpGet("Journeys")]
        //public async Task<ActionResult<List<JourneyDto>>> GetJourneys([FromQuery] string projectName)
        //{
        //    var result = await _mediator.Send(new GetUniqueTagJourneys|Query(projectName));
        //    return this.FromResult(result);
        //}
    }
}
