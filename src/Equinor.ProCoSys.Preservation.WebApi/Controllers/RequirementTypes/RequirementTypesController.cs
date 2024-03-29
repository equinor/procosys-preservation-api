﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.CreateRequirementType;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.DeleteRequirementDefinition;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.DeleteRequirementType;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementDefinition;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementType;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UpdateRequirementType;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.VoidRequirementDefinition;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.VoidRequirementType;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Query.GetAllRequirementTypes;
using Equinor.ProCoSys.Preservation.Query.GetRequirementTypeById;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.RequirementTypes
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
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetAllRequirementTypesQuery(includeVoided));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_READ)]
        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementTypeDetailsDto>> GetRequirementType(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetRequirementTypeByIdQuery(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPost]
        public async Task<ActionResult<int>> CreateRequirementType(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] CreateRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new CreateRequirementTypeCommand(dto.SortKey, dto.Code, dto.Title, dto.Icon));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}")]
        public async Task<ActionResult<RequirementTypeDto>> UpdateRequirementType(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] UpdateRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new UpdateRequirementTypeCommand(id, dto.RowVersion, dto.SortKey, dto.Title, dto.Code, dto.Icon));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/Void")]
        public async Task<ActionResult<RequirementTypeDto>> VoidRequirementType(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] VoidRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new VoidRequirementTypeCommand(id, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/Unvoid")]
        public async Task<ActionResult<RequirementTypeDto>> UnvoidRequirementType(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] UnvoidRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new UnvoidRequirementTypeCommand(id, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_DELETE)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRequirementType(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] DeleteRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new DeleteRequirementTypeCommand(id, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPost("{id}/RequirementDefinitions")]
        public async Task<ActionResult<int>> CreateRequirementDefinition(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] CreateRequirementDefinitionDto dto)
        {
            var fields = dto.Fields?.Select(f =>
                new FieldsForCommand(f.Label, f.FieldType, f.SortKey, f.Unit, f.ShowPrevious)).ToList();
            var result = await _mediator.Send(new CreateRequirementDefinitionCommand(id, dto.SortKey, dto.Usage,
                dto.Title, dto.DefaultIntervalWeeks, fields));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/RequirementDefinitions/{requirementDefinitionId}/Void")]
        public async Task<ActionResult<RequirementDefinitionDto>> VoidRequirementDefinition(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/RequirementDefinitions/{requirementDefinitionId}/Unvoid")]
        public async Task<ActionResult<RequirementDefinitionDto>> UnvoidRequirementDefinition(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_DELETE)]
        [HttpDelete("{id}/RequirementDefinitions/{requirementDefinitionId}")]
        public async Task<ActionResult> DeleteRequirementDefinition(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementDefinitionId,
            [FromBody] DeleteRequirementDefinitionDto dto)
        {
            var result = await _mediator.Send(new DeleteRequirementDefinitionCommand(id, requirementDefinitionId, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}/RequirementDefinitions/{requirementDefinitionId}")]
        public async Task<ActionResult<RequirementDefinitionDto>> UpdateRequirementDefinition(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementDefinitionId,
            [FromBody] UpdateRequirementDefinitionDto dto)
        {
            var updatedFields = dto.UpdatedFields?.Select(f =>
                new UpdateFieldsForCommand(
                    f.Id, 
                    f.Label, 
                    f.FieldType, 
                    f.SortKey, 
                    f.IsVoided,
                    f.RowVersion, 
                    f.Unit, 
                    f.ShowPrevious)).ToList();
            var newFields = dto.NewFields?.Select(f =>
                new FieldsForCommand(
                    f.Label, 
                    f.FieldType, 
                    f.SortKey, 
                    f.Unit, 
                    f.ShowPrevious)).ToList();

            var command = new UpdateRequirementDefinitionCommand(
                id,
                requirementDefinitionId,
                dto.SortKey,
                dto.Usage,
                dto.Title,
                dto.DefaultIntervalWeeks,
                dto.RowVersion,
                updatedFields,
                newFields);
            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }
    }
}
