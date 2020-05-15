using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidJourney;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.Procosys.Preservation.Command.JourneyCommands.VoidJourney;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.JourneyAggregate;
using Equinor.Procosys.Preservation.WebApi.Misc;
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
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetAllJourneysQuery(includeVoided));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_READ)]
        [HttpGet("{id}")]
        public async Task<ActionResult<JourneyDto>> GetJourney(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetJourneyByIdQuery(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPost]
        public async Task<ActionResult<int>> AddJourney(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] CreateJourneyDto dto)
        {
            var result = await _mediator.Send(new CreateJourneyCommand(dto.Title));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPost("{id}/AddStep")]
        public async Task<ActionResult> AddStep(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] CreateStepDto dto)
        {
            var result = await _mediator.Send(new CreateStepCommand(id, dto.Title, dto.ModeId, dto.ResponsibleId));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJourney(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] UpdateJourneyDto dto)
        {
            var result = await _mediator.Send(new UpdateJourneyCommand(id, dto.Title, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}/Steps/{stepId}")]
        public async Task<ActionResult> UpdateStep(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] UpdateStepDto dto)
        {
            var result = await _mediator.Send(new UpdateStepCommand(id, dto.Title, dto.Rowversion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/Void")]
        public async Task<IActionResult> VoidJourney(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] VoidJourneyDto dto)
        {
            var result = await _mediator.Send(new VoidJourneyCommand(id, dto.RowVersion));

            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/Unvoid")]
        public async Task<IActionResult> UnvoidJourney(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] UnvoidJourneyDto dto)
        {
            var result = await _mediator.Send(new UnvoidJourneyCommand(id, dto.RowVersion));

            return this.FromResult(result);
        }
    }
}
