using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Command.TagFunctionCommands.UnvoidTagFunction;
using Equinor.Procosys.Preservation.Command.TagFunctionCommands.UpdateRequirements;
using Equinor.Procosys.Preservation.Command.TagFunctionCommands.VoidTagFunction;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.GetTagFunctionDetails;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_READ)]
        [HttpGet("{code}")]
        public async Task<ActionResult<TagFunctionDetailsDto>> GetTagFunctionDetails(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] string code,
            [FromQuery] string registerCode)
        {
            var result = await _mediator.Send(new GetTagFunctionDetailsQuery(code, registerCode));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut]
        public async Task<ActionResult> UpdateRequirements(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] UpdateRequirementsDto dto)
        {
            var requirements = dto.Requirements?
                .Select(r =>
                    new RequirementForCommand(r.RequirementDefinitionId, r.IntervalWeeks));
            var result = await _mediator.Send(
                new UpdateRequirementsCommand(
                    dto.TagFunctionCode,
                    dto.RegisterCode,
                    requirements,
                    dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{code}/Void")]
        public async Task<ActionResult<TagFunctionDetailsDto>> VoidTagFunction(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] string code,
            [FromBody] VoidTagFunctionDto dto)
        {
            var result = await _mediator.Send(new VoidTagFunctionCommand(code, dto.RegisterCode, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{code}/Unvoid")]
        public async Task<ActionResult<TagFunctionDetailsDto>> UnvoidTagFunction(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] string code,
            [FromBody] UnvoidTagFunctionDto dto)
        {
            var result = await _mediator.Send(new UnvoidTagFunctionCommand(code, dto.RegisterCode, dto.RowVersion));
            return this.FromResult(result);
        }
    }
}
