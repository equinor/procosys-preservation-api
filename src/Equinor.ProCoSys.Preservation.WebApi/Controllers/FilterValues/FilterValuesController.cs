﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagAreas;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagDisciplines;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagFunctions;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagJourneys;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagModes;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagRequirementTypes;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagResponsibles;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.FilterValues
{
    [ApiController]
    [Route("FilterValues")]
    public class FilterValuesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FilterValuesController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("RequirementTypes")]
        public async Task<ActionResult<List<RequirementTypeDto>>> GetRequirementTypes(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagRequirementTypesQuery(projectName));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("Responsibles")]
        public async Task<ActionResult<List<ResponsibleDto>>> GetResponsibles(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagResponsiblesQuery(projectName));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("Journeys")]
        public async Task<ActionResult<List<JourneyDto>>> GetJourneys(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagJourneysQuery(projectName));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("Modes")]
        public async Task<ActionResult<List<ModeDto>>> GetModes(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagModesQuery(projectName));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("Areas")]
        public async Task<ActionResult<List<AreaDto>>> GetAreas(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagAreasQuery(projectName));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("TagFunctions")]
        public async Task<ActionResult<List<TagFunctionCodeDto>>> GetTagFunctions(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagFunctionsQuery(projectName));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("Disciplines")]
        public async Task<ActionResult<List<DisciplineDto>>> GetTagDisciplines(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string projectName)
        {
            var result = await _mediator.Send(new GetUniqueTagDisciplinesQuery(projectName));
            return this.FromResult(result);
        }
    }
}
