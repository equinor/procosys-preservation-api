﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementDefinition;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementType;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementDefinition;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementType;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.GetRequirementType;
using Equinor.Procosys.Preservation.Query.GetRequirementTypes;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequirementDefinitionDto = Equinor.Procosys.Preservation.Query.GetRequirementType.RequirementDefinitionDto;
using RequirementTypeDto = Equinor.Procosys.Preservation.Query.GetRequirementType.RequirementTypeDto;

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
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetAllRequirementTypesQuery(includeVoided));
            return Ok(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_READ)]
        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementTypeDto>> GetRequirementType(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetRequirementTypeByIdQuery(id));
            return Ok(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}/Void")]
        public async Task<ActionResult<RequirementTypeDto>> VoidRequirementType(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] VoidRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new VoidRequirementTypeCommand(id, dto.RowVersion));
            return Ok(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}/Unvoid")]
        public async Task<ActionResult<RequirementTypeDto>> UnvoidRequirementType(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] UnvoidRequirementTypeDto dto)
        {
            var result = await _mediator.Send(new UnvoidRequirementTypeCommand(id, dto.RowVersion));
            return Ok(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}/RequirementDefinitions/{requirementDefinitionId}/Void")]
        public async Task<ActionResult<RequirementDefinitionDto>> VoidRequirementDefinition(
            [FromHeader( Name = PlantProvider.PlantHeader)]
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

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}/RequirementDefinitions/{requirementDefinitionId}/Unvoid")]
        public async Task<ActionResult<RequirementDefinitionDto>> UnvoidRequirementDefinition(
            [FromHeader( Name = PlantProvider.PlantHeader)]
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
