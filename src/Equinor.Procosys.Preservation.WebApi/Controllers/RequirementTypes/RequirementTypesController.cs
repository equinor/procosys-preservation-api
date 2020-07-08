using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementType;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.DeleteRequirementType;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementDefinition;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementType;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementDefinition;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementType;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    [ApiController]
    [Route("RequirementTypes")]
    public class RequirementTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequirementTypesController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_READ)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequirementTypeDto>>> GetRequirementTypes(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetAllRequirementTypesQuery(includeVoided));
            return Ok(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_READ)]
        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementTypeDto>> GetRequirementType(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetRequirementTypeByIdQuery(id));
            return Ok(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPost]
        public async Task<ActionResult<int>> CreateRequirementType(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] CreateRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new CreateRequirementTypeCommand(dto.SortKey, dto.Code, dto.Title));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/Void")]
        public async Task<ActionResult<RequirementTypeDto>> VoidRequirementType(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] VoidRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new VoidRequirementTypeCommand(id, dto.RowVersion));
            return Ok(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/Unvoid")]
        public async Task<ActionResult<RequirementTypeDto>> UnvoidRequirementType(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] UnvoidRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new UnvoidRequirementTypeCommand(id, dto.RowVersion));
            return Ok(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_DELETE)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRequirementType(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] DeleteRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new DeleteRequirementTypeCommand(id, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/RequirementDefinitions/{requirementDefinitionId}/Void")]
        public async Task<ActionResult<RequirementDefinitionDto>> VoidRequirementDefinition(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementDefinitionId,
            [FromBody] VoidRequirementDefinitionDto dto)
        {
            var command = new VoidRequirementDefinitionCommand(
                id,
                requirementDefinitionId,
                dto.RowVersion);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/RequirementDefinitions/{requirementDefinitionId}/Unvoid")]
        public async Task<ActionResult<RequirementDefinitionDto>> UnvoidRequirementDefinition(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementDefinitionId,
            [FromBody] UnvoidRequirementDefinitionDto dto)
        {
            var command = new UnvoidRequirementDefinitionCommand(
                id,
                requirementDefinitionId,
                dto.RowVersion);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
