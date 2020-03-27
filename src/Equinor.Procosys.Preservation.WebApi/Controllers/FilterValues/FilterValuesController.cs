using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetUniqueTagAreas;
using Equinor.Procosys.Preservation.Query.GetUniqueTagDisciplines;
using Equinor.Procosys.Preservation.Query.GetUniqueTagFunctions;
using Equinor.Procosys.Preservation.Query.GetUniqueTagJourneys;
using Equinor.Procosys.Preservation.Query.GetUniqueTagModes;
using Equinor.Procosys.Preservation.Query.GetUniqueTagRequirementTypes;
using Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibles;
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
        public async Task<ActionResult<List<RequirementTypeDto>>> GetRequirementTypes([FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagRequirementTypesQuery(projectName));
            return this.FromResult(result);
        }

        [HttpGet("Responsibles")]
        public async Task<ActionResult<List<ResponsibleDto>>> GetResponsibles([FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagResponsiblesQuery(projectName));
            return this.FromResult(result);
        }

        [HttpGet("Journeys")]
        public async Task<ActionResult<List<JourneyDto>>> GetJourneys([FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagJourneysQuery(projectName));
            return this.FromResult(result);
        }

        [HttpGet("Modes")]
        public async Task<ActionResult<List<ModeDto>>> GetModes([FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagModesQuery(projectName));
            return this.FromResult(result);
        }

        [HttpGet("Areas")]
        public async Task<ActionResult<List<AreaDto>>> GetAreas([FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagAreasQuery(projectName));
            return this.FromResult(result);
        }

        [HttpGet("TagFunctions")]
        public async Task<ActionResult<List<TagFunctionCodeDto>>> GetTagFunctions([FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagFunctionsQuery(projectName));
            return this.FromResult(result);
        }

        [HttpGet("Disciplines")]
        public async Task<ActionResult<List<DisciplineDto>>> GetTagDisciplines([FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagDisciplinesQuery(projectName));
            return this.FromResult(result);
        }
    }
}
