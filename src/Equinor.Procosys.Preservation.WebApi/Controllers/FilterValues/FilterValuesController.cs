using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.GetUniqueTagAreas;
using Equinor.Procosys.Preservation.Query.GetUniqueTagDisciplines;
using Equinor.Procosys.Preservation.Query.GetUniqueTagFunctions;
using Equinor.Procosys.Preservation.Query.GetUniqueTagJourneys;
using Equinor.Procosys.Preservation.Query.GetUniqueTagModes;
using Equinor.Procosys.Preservation.Query.GetUniqueTagRequirementTypes;
using Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibles;
using Equinor.Procosys.Preservation.WebApi.Misc;
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
        public async Task<ActionResult<List<RequirementTypeDto>>> GetRequirementTypes(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagRequirementTypesQuery(plant, projectName));
            return this.FromResult(result);
        }

        [HttpGet("Responsibles")]
        public async Task<ActionResult<List<ResponsibleDto>>> GetResponsibles(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagResponsiblesQuery(plant, projectName));
            return this.FromResult(result);
        }

        [HttpGet("Journeys")]
        public async Task<ActionResult<List<JourneyDto>>> GetJourneys(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagJourneysQuery(plant, projectName));
            return this.FromResult(result);
        }

        [HttpGet("Modes")]
        public async Task<ActionResult<List<ModeDto>>> GetModes(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagModesQuery(plant, projectName));
            return this.FromResult(result);
        }

        [HttpGet("Areas")]
        public async Task<ActionResult<List<AreaDto>>> GetAreas(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagAreasQuery(plant, projectName));
            return this.FromResult(result);
        }

        [HttpGet("TagFunctions")]
        public async Task<ActionResult<List<TagFunctionCodeDto>>> GetTagFunctions(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagFunctionsQuery(plant, projectName));
            return this.FromResult(result);
        }

        [HttpGet("Disciplines")]
        public async Task<ActionResult<List<DisciplineDto>>> GetTagDisciplines(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagDisciplinesQuery(plant, projectName));
            return this.FromResult(result);
        }
    }
}
