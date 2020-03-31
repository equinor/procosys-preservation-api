using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Command.TagFunctionCommands.UpdateRequirements;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.GetTagFunctionDetails;
using Equinor.Procosys.Preservation.Query.GetTagFunctionsHavingRequirement;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.TagFunctions
{
    [ApiController]
    [Route("TagFunctions")]
    public class TagFunctionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagFunctionsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("HavingRequirement")]
        public async Task<ActionResult<IEnumerable<TagFunctionDto>>> GetTagFunctionsHavingRequirement(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant)
        {
            var result = await _mediator.Send(new GetTagFunctionsHavingRequirementQuery(plant));
            return this.FromResult(result);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<TagFunctionDetailsDto>> GetTagFunctionDetails(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] string code,
            [FromQuery] string registerCode)
        {
            var result = await _mediator.Send(new GetTagFunctionDetailsQuery(plant, code, registerCode));
            return this.FromResult(result);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateRequirements(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromBody] UpdateRequirementsDto dto)
        {
            var requirements = dto.Requirements?
                .Select(r =>
                    new Requirement(r.RequirementDefinitionId, r.IntervalWeeks));
            var result = await _mediator.Send(
                new UpdateRequirementsCommand(
                    plant,
                    dto.TagFunctionCode,
                    dto.RegisterCode,
                    requirements));
            return this.FromResult(result);
        }
    }
}
