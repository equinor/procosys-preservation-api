using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues;
using Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.GetTagActionDetails;
using Equinor.Procosys.Preservation.Query.GetTagActions;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Equinor.Procosys.Preservation.Query.GetTagRequirements;
using Equinor.Procosys.Preservation.Query.GetTags;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;
using RequirementDto = Equinor.Procosys.Preservation.Query.GetTagRequirements.RequirementDto;
using RequirementPreserveCommand = Equinor.Procosys.Preservation.Command.RequirementCommands.Preserve.PreserveCommand;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    /// <summary>
    /// Handles requests that deal with preservation tags
    /// </summary>
    [ApiController]
    [Route("Tags")]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromQuery] FilterDto filter,
            [FromQuery] SortingDto sorting,
            [FromQuery] PagingDto paging)
        {
            var query = CreateGetTagsQuery(plant, filter, sorting, paging);

            var result = await _mediator.Send(query);
            return this.FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDetailsDto>> GetTagDetails(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagDetailsQuery(plant, id));
            return this.FromResult(result);
        }

        [HttpGet("{id}/Requirements")]
        public async Task<ActionResult<List<RequirementDto>>> GetTagRequirements(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagRequirementsQuery(plant, id));
            return this.FromResult(result);
        }

        [HttpGet("{id}/Actions")]
        public async Task<ActionResult<List<ActionDto>>> GetTagActions(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagActionsQuery(plant, id));
            return this.FromResult(result);
        }

        [HttpGet("{id}/Actions/{actionId}")]
        public async Task<ActionResult<ActionDetailsDto>> GetTagActionDetails(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int actionId)
        {
            var result = await _mediator.Send(new GetActionDetailsQuery(plant, id, actionId));
            return this.FromResult(result);
        }

        [HttpPost("Standard")]
        public async Task<ActionResult<int>> CreateTag(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromBody] CreateTagDto dto)
        {
            var requirements = dto.Requirements?
                .Select(r =>
                    new Requirement(r.RequirementDefinitionId, r.IntervalWeeks));
            var result = await _mediator.Send(
                new CreateTagCommand(
                    plant,
                    dto.TagNos,
                    dto.ProjectName,
                    dto.StepId,
                    requirements,
                    dto.Remark,
                    dto.StorageArea));
            return this.FromResult(result);
        }

        [HttpPost("Area")]
        public async Task<ActionResult<int>> CreateAreaTag(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromBody] CreateAreaTagDto dto)
        {
            var requirements = dto.Requirements?
                .Select(r =>
                    new Requirement(r.RequirementDefinitionId, r.IntervalWeeks));
            
            var result = await _mediator.Send(
                new CreateAreaTagCommand(
                    plant,
                    dto.ProjectName,
                    dto.AreaTagType.ConvertToTagType(),
                    dto.DisciplineCode,
                    dto.AreaCode,
                    dto.TagNoSuffix,
                    dto.StepId,
                    requirements,
                    dto.Description,
                    dto.Remark,
                    dto.StorageArea));

            return this.FromResult(result);
        }

        [HttpPut("{id}/StartPreservation")]
        public async Task<IActionResult> StartPreservation(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new StartPreservationCommand(plant, new List<int>{id}));
            return this.FromResult(result);
        }

        [HttpPut("StartPreservation")]
        public async Task<IActionResult> StartPreservation(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new StartPreservationCommand(plant, tagIds));
            return this.FromResult(result);
        }

        [HttpPut("{id}/Preserve")]
        public async Task<IActionResult> Preserve(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new PreserveCommand(plant, id));
            return this.FromResult(result);
        }

        [HttpPut("BulkPreserve")]
        public async Task<IActionResult> BulkPreserve(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new BulkPreserveCommand(plant, tagIds));
            return this.FromResult(result);
        }

        [HttpPut("Transfer")]
        public async Task<IActionResult> Transfer(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new TransferCommand(plant, tagIds));
            return this.FromResult(result);
        }

        [HttpPost("{id}/Requirement/{requirementId}/RecordValues")]
        public async Task<IActionResult> RecordCheckBoxChecked(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementId,
            [FromBody] RequirementValuesDto requirementValuesDto)
        {
            var fieldValues = requirementValuesDto?
                .FieldValues
                .ToDictionary(
                    keySelector => keySelector.FieldId,
                    elementSelector => elementSelector.Value);

            var result = await _mediator.Send(new RecordValuesCommand(plant, id, requirementId, fieldValues, requirementValuesDto?.Comment));
            
            return this.FromResult(result);
        }

        [HttpPost("{id}/Requirement/{requirementId}/Preserve")]
        public async Task<IActionResult> Preserve(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementId)
        {
            var result = await _mediator.Send(new RequirementPreserveCommand(plant, id, requirementId));
            
            return this.FromResult(result);
        }

        private static GetTagsQuery CreateGetTagsQuery(string plant, FilterDto filter, SortingDto sorting, PagingDto paging)
        {
            var query = new GetTagsQuery(
                plant,
                filter.ProjectName,
                new Sorting(sorting.Direction, sorting.Property),
                new Filter(),
                new Paging(paging.Page, paging.Size)
            );

            if (filter.ActionStatus.HasValue)
            {
                query.Filter.ActionStatus = filter.ActionStatus;
            }

            if (filter.DueFilters != null)
            {
                query.Filter.DueFilters = filter.DueFilters;
            }

            if (filter.PreservationStatus.HasValue)
            {
                query.Filter.PreservationStatus = filter.PreservationStatus;
            }

            if (filter.RequirementTypeIds != null)
            {
                query.Filter.RequirementTypeIds = filter.RequirementTypeIds;
            }

            if (filter.AreaCodes != null)
            {
                query.Filter.AreaCodes = filter.AreaCodes;
            }

            if (filter.DisciplineCodes != null)
            {
                query.Filter.DisciplineCodes = filter.DisciplineCodes;
            }

            if (filter.ResponsibleIds != null)
            {
                query.Filter.ResponsibleIds = filter.ResponsibleIds;
            }

            if (filter.TagFunctionCodes != null)
            {
                query.Filter.TagFunctionCodes = filter.TagFunctionCodes;
            }

            if (filter.ModeIds != null)
            {
                query.Filter.ModeIds = filter.ModeIds;
            }

            if (filter.JourneyIds != null)
            {
                query.Filter.JourneyIds = filter.JourneyIds;
            }

            if (filter.StepIds != null)
            {
                query.Filter.StepIds = filter.StepIds;
            }

            if (filter.TagNoStartsWith != null)
            {
                query.Filter.TagNoStartsWith = filter.TagNoStartsWith;
            }

            if (filter.CommPkgNoStartsWith != null)
            {
                query.Filter.CommPkgNoStartsWith = filter.CommPkgNoStartsWith;
            }

            if (filter.McPkgNoStartsWith != null)
            {
                query.Filter.McPkgNoStartsWith = filter.McPkgNoStartsWith;
            }

            if (filter.PurchaseOrderNoStartsWith != null)
            {
                query.Filter.PurchaseOrderNoStartsWith = filter.PurchaseOrderNoStartsWith;
            }

            if (filter.StorageAreaStartsWith != null)
            {
                query.Filter.StorageAreaStartsWith = filter.StorageAreaStartsWith;
            }

            if (filter.CallOffStartsWith != null)
            {
                query.Filter.CallOffStartsWith = filter.CallOffStartsWith;
            }

            return query;
        }
    }
}
