using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep;
using Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteJourney;
using Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteStep;
using Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidJourney;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.Procosys.Preservation.Command.JourneyCommands.VoidJourney;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.GetAllJourneys;
using Equinor.Procosys.Preservation.Query.GetJourneyById;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    [ApiController]
    [Route("Journeys")]
    public class JourneysController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JourneysController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_READ)]
        [HttpGet]
        public async Task<ActionResult<List<JourneyDto>>> GetJourneys(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetAllJourneysQuery(includeVoided));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_READ)]
        [HttpGet("{id}")]
        public async Task<ActionResult<JourneyDetailsDto>> GetJourney(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetJourneyByIdQuery(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPost]
        public async Task<ActionResult<int>> AddJourney(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] CreateJourneyDto dto)
        {
            var result = await _mediator.Send(new CreateJourneyCommand(dto.Title));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJourney(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] UpdateJourneyDto dto)
        {
            var result = await _mediator.Send(new UpdateJourneyCommand(id, dto.Title, dto.RowVersion));
            return this.FromResult(result);
        }
        
        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/Void")]
        public async Task<IActionResult> VoidJourney(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] VoidJourneyDto dto)
        {
            var result = await _mediator.Send(new VoidJourneyCommand(id, dto.RowVersion));

            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/Unvoid")]
        public async Task<IActionResult> UnvoidJourney(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] UnvoidJourneyDto dto)
        {
            var result = await _mediator.Send(new UnvoidJourneyCommand(id, dto.RowVersion));

            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_DELETE)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteJourney(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] DeleteJourneyDto dto)
        {
            var result = await _mediator.Send(new DeleteJourneyCommand(id, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPost("{id}/AddStep")]
        public async Task<ActionResult> AddStep(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] CreateStepDto dto)
        {
            var result = await _mediator.Send(new CreateStepCommand(id, dto.Title, dto.ModeId, dto.ResponsibleCode));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}/Steps/{stepId}")]
        public async Task<ActionResult> UpdateStep(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromRoute] int stepId,
            [FromBody] UpdateStepDto dto)
        {
            var command = new UpdateStepCommand(
                id,
                stepId,
                dto.ModeId,
                dto.ResponsibleCode,
                dto.Title,
                dto.RowVersion);
            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }
        
        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_DELETE)]
        [HttpDelete("{id}/Steps/{stepId}")]
        public async Task<ActionResult> DeleteStep(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromRoute] int stepId,
            [FromBody] DeleteStepDto dto)
        {
            var result = await _mediator.Send(new DeleteStepCommand(id, stepId, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}/Steps/SwapSteps")]
        public async Task<ActionResult> SwapSteps(
            [FromHeader( Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] PairedStepIdWithRowVersionDto pairedStepsDto)
        {
            var command = new SwapStepsCommand(
                id, pairedStepsDto.StepA.Id, 
                pairedStepsDto.StepA.RowVersion, 
                pairedStepsDto.StepB.Id,
                pairedStepsDto.StepB.RowVersion
                );
            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }
    }
}
