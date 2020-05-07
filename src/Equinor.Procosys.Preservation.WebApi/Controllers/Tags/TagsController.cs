using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction;
using Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction;
using Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues;
using Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload;
using Equinor.Procosys.Preservation.Command.TagCommands.AutoScopeTags;
using Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTags;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation;
using Equinor.Procosys.Preservation.Command.TagCommands.CompletePreservation;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using Equinor.Procosys.Preservation.Command.TagCommands.UnvoidTag;
using Equinor.Procosys.Preservation.Command.TagCommands.UpdateTag;
using Equinor.Procosys.Preservation.Command.TagCommands.VoidTag;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.CheckAreaTagNo;
using Equinor.Procosys.Preservation.Query.GetTagActionDetails;
using Equinor.Procosys.Preservation.Query.GetTagActions;
using Equinor.Procosys.Preservation.Query.GetTagAttachments;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Equinor.Procosys.Preservation.Query.GetTagRequirements;
using Equinor.Procosys.Preservation.Query.GetTags;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromQuery] FilterDto filter,
            [FromQuery] SortingDto sorting,
            [FromQuery] PagingDto paging)
        {
            var query = CreateGetTagsQuery(filter, sorting, paging);

            var result = await _mediator.Send(query);
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("{id}")]
        public async Task<ActionResult<TagDetailsDto>> GetTagDetails(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagDetailsQuery(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("{id}/Requirements")]
        public async Task<ActionResult<List<RequirementDto>>> GetTagRequirements(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagRequirementsQuery(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("{id}/Actions")]
        public async Task<ActionResult<List<ActionDto>>> GetTagActions(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagActionsQuery(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("{id}/Actions/{actionId}")]
        public async Task<ActionResult<ActionDetailsDto>> GetTagActionDetails(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int actionId)
        {
            var result = await _mediator.Send(new GetActionDetailsQuery(id, actionId));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_CREATE)]
        [HttpPost("{id}/Actions")]
        public async Task<ActionResult<int>> CreateAction(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] CreateActionDto dto)
        {
            var actionCommand = new CreateActionCommand(
                    id,
                    dto.Title,
                    dto.Description,
                    dto.DueTimeUtc);

            var result = await _mediator.Send(actionCommand);

            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpPut("{id}/Actions/{actionId}")]
        public async Task<IActionResult> UpdateAction(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int actionId,
            [FromBody] UpdateActionDto dto)
        {
                var actionCommand = new UpdateActionCommand(
                                  id,
                                  actionId,
                                  dto.Title,
                                  dto.Description,
                                  dto.DueTimeUtc);

                var result = await _mediator.Send(actionCommand);

                return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] UpdateTagDto dto)
        {
            var result = await _mediator.Send(
                new UpdateTagCommand(id,
                    dto.Remark,
                    dto.StorageArea));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_CREATE)]
        [HttpPost("Standard")]
        public async Task<ActionResult<int>> CreateTags(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] CreateTagsDto dto)
        {
            var requirements = dto.Requirements?
                .Select(r =>
                    new RequirementForCommand(r.RequirementDefinitionId, r.IntervalWeeks));
            var result = await _mediator.Send(
                new CreateTagsCommand(
                    dto.TagNos,
                    dto.ProjectName,
                    dto.StepId,
                    requirements,
                    dto.Remark,
                    dto.StorageArea));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_CREATE)]
        [HttpPost("AutoScope")]
        public async Task<ActionResult<int>> AutoScopeTags(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] AutoScopeTagsDto dto)
        {
            var result = await _mediator.Send(
                new AutoScopeTagsCommand(
                    dto.TagNos,
                    dto.ProjectName,
                    dto.StepId,
                    dto.Remark,
                    dto.StorageArea));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_CREATE)]
        [HttpPost("Area")]
        public async Task<ActionResult<int>> CreateAreaTag(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] CreateAreaTagDto dto)
        {
            var requirements = dto.Requirements?
                .Select(r =>
                    new RequirementForCommand(r.RequirementDefinitionId, r.IntervalWeeks));
            
            var result = await _mediator.Send(
                new CreateAreaTagCommand(
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

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("CheckAreaTagNo")]
        public async Task<ActionResult<Query.CheckAreaTagNo.AreaTagDto>> CheckAreaTagNo(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromQuery] AreaTagDto dto)
        {
            var result = await _mediator.Send(
                new CheckAreaTagNoQuery(
                    dto.ProjectName,
                    dto.AreaTagType.ConvertToTagType(),
                    dto.DisciplineCode,
                    dto.AreaCode,
                    dto.TagNoSuffix));

            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/StartPreservation")]
        public async Task<IActionResult> StartPreservation(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new StartPreservationCommand(new List<int>{id}));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("StartPreservation")]
        public async Task<IActionResult> StartPreservation(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new StartPreservationCommand(tagIds));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpPut("{id}/Preserve")]
        public async Task<IActionResult> Preserve(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new PreserveCommand(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpPut("BulkPreserve")]
        public async Task<IActionResult> BulkPreserve(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new BulkPreserveCommand(tagIds));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("Transfer")]
        public async Task<IActionResult> Transfer(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new TransferCommand(tagIds));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/CompletePreservation")]
        public async Task<IActionResult> CompletePreservation(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new CompletePreservationCommand(new List<int> { id }));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("CompletePreservation")]
        public async Task<IActionResult> CompletePreservation(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new CompletePreservationCommand(tagIds));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpPost("{id}/Requirement/{requirementId}/RecordValues")]
        public async Task<IActionResult> RecordValues(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementId,
            [FromBody] RequirementValuesDto requirementValuesDto)
        {
            var numberValues = requirementValuesDto?
                .NumberValues?
                .Select(fv => new NumberFieldValue(fv.FieldId, fv.Value, fv.IsNA)).ToList();
            var checkBoxValues = requirementValuesDto?
                .CheckBoxValues?
                .Select(fv => new CheckBoxFieldValue(fv.FieldId, fv.IsChecked)).ToList();

            var result = await _mediator.Send(
                new RecordValuesCommand(
                    id,
                    requirementId,
                    numberValues,
                    checkBoxValues,
                    requirementValuesDto?.Comment));
            
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpPost("{id}/Requirement/{requirementId}/Preserve")]
        public async Task<IActionResult> Preserve(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementId)
        {
            var result = await _mediator.Send(new RequirementPreserveCommand(id, requirementId));
            
            return this.FromResult(result);
        }
        
        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("{id}/Attachments")]
        public async Task<ActionResult<List<TagAttachmentDto>>> GetTagAttachments(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagAttachmentsQuery(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_ATTACHFILE)]
        [HttpPost("{id}/Attachments")]
        public async Task<ActionResult<int>> UploadTagAttachment(
            [FromHeader(Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromForm] UploadAttachmentDto dto)
        {
            //using (var memStream = new MemoryStream())
            //{
            //    await dto.File.CopyToAsync(memStream);

            //    var actionCommand = new UploadTagAttachmentCommand(
            //        id,
            //        memStream,
            //        dto.File.FileName,
            //        dto.Title,
            //        dto.OverwriteIfExists);

            //    var result = await _mediator.Send(actionCommand);
            //    return this.FromResult(result);
            //}
            await using (var stream = dto.File.OpenReadStream())
            {
                var actionCommand = new UploadTagAttachmentCommand(
                    id,
                    stream,
                    "Preservation_Privileges.xlsx",
                    dto.Title,
                    dto.OverwriteIfExists);

                var result = await _mediator.Send(actionCommand);
                return this.FromResult(result);
            }
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/VoidTag")]
        public async Task<IActionResult> VoidTag(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new VoidTagCommand(id));

            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/UnvoidTag")]
        public async Task<IActionResult> UnvoidTag(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new UnvoidTagCommand(id));

            return this.FromResult(result);
        }

        private static GetTagsQuery CreateGetTagsQuery(FilterDto filter, SortingDto sorting, PagingDto paging)
        {
            var query = new GetTagsQuery(
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
