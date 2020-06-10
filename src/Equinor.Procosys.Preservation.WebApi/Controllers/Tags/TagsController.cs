using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Delete;
using Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Upload;
using Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction;
using Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction;
using Equinor.Procosys.Preservation.Command.ActionCommands.CloseAction;
using Equinor.Procosys.Preservation.Command.RequirementCommands.DeleteAttachment;
using Equinor.Procosys.Preservation.Command.RequirementCommands.Upload;
using Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues;
using Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload;
using Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Delete;
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
using Equinor.Procosys.Preservation.Query.GetActionAttachment;
using Equinor.Procosys.Preservation.Query.GetActionAttachments;
using Equinor.Procosys.Preservation.Query.GetActionDetails;
using Equinor.Procosys.Preservation.Query.GetActions;
using Equinor.Procosys.Preservation.Query.GetFieldValueAttachment;
using Equinor.Procosys.Preservation.Query.GetTagAttachment;
using Equinor.Procosys.Preservation.Query.GetTagAttachments;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Equinor.Procosys.Preservation.Query.GetPreservationRecords;
using Equinor.Procosys.Preservation.Query.GetTagRequirements;
using Equinor.Procosys.Preservation.Query.GetTags;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult;
using ServiceResult.ApiExtensions;
using PreservationRecordDto = Equinor.Procosys.Preservation.Query.GetPreservationRecords.PreservationRecordDto;
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
        public async Task<ActionResult<List<TagRequirementDto>>> GetTagRequirements(
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
            var result = await _mediator.Send(new GetActionsQuery(id));
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
            var command = new CreateActionCommand(
                    id,
                    dto.Title,
                    dto.Description,
                    dto.DueTimeUtc);

            var result = await _mediator.Send(command);

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
                var command = new UpdateActionCommand(
                                  id,
                                  actionId,
                                  dto.Title,
                                  dto.Description,
                                  dto.DueTimeUtc,
                                  dto.RowVersion);

                var result = await _mediator.Send(command);

                return this.FromResult(result);
        }
         
        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpPut("{id}/Actions/{actionId}/Close")]
        public async Task<IActionResult> CloseAction(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int actionId,
            [FromBody] CloseActionDto dto)
        {
            var command = new CloseActionCommand(
                id,
                actionId,
                dto.RowVersion);

            var result = await _mediator.Send(command);

            return this.FromResult(result);
        }
       
        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("{id}/Actions/{actionId}/Attachments")]
        public async Task<ActionResult<List<ActionAttachmentDto>>> GetActionAttachments(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int actionId)
        {
            var result = await _mediator.Send(new GetActionAttachmentsQuery(id, actionId));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("{id}/Actions/{actionId}/Attachments/{attachmentId}")]
        public async Task<ActionResult> GetActionAttachment(
            [FromHeader(Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int actionId,
            [FromRoute] int attachmentId,
            [FromQuery] bool redirect = false)
        {
            var result = await _mediator.Send(new GetActionAttachmentQuery(id, actionId, attachmentId));

            if (result.ResultType != ResultType.Ok)
            {
                return this.FromResult(result);
            }

            if (!redirect)
            {
                return Ok(result.Data.ToString());
            }

            return Redirect(result.Data.ToString());
        }

        [Authorize(Roles = Permissions.PRESERVATION_ATTACHFILE)]
        [HttpPost("{id}/Actions/{actionId}/Attachments")]
        public async Task<ActionResult<int>> UploadActionAttachment(
            [FromHeader(Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int actionId,
            [FromForm] UploadAttachmentWithOverwriteOptionDto dto)
        {
            await using var stream = dto.File.OpenReadStream();

            var command = new UploadActionAttachmentCommand(
                id,
                actionId,
                dto.File.FileName,
                dto.OverwriteIfExists,
                stream);

            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_DETACHFILE)]
        [HttpDelete("{id}/Actions/{actionId}/Attachments/{attachmentId}")]
        public async Task<ActionResult<int>> DeleteActionAttachment(
            [FromHeader(Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int actionId,
            [FromRoute] int attachmentId,
            [FromBody] DeleteActionAttachmentDto dto)
        {
            var actionCommand = new DeleteActionAttachmentCommand(
                id,
                actionId,
                attachmentId,
                dto.RowVersion);

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
            var command = new UpdateTagCommand(id,
                dto.Remark,
                dto.StorageArea,
                dto.RowVersion);
            
            var result = await _mediator.Send(command);
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
            var command = new CreateTagsCommand(
                dto.TagNos,
                dto.ProjectName,
                dto.StepId,
                requirements,
                dto.Remark,
                dto.StorageArea);
            
            var result = await _mediator.Send(command);
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

            var command = new CreateAreaTagCommand(
                dto.ProjectName,
                dto.AreaTagType.ConvertToTagType(),
                dto.DisciplineCode,
                dto.AreaCode,
                dto.PurchaseOrderCalloffCode,
                dto.TagNoSuffix,
                dto.StepId,
                requirements,
                dto.Description,
                dto.Remark,
                dto.StorageArea);

            var result = await _mediator.Send(command);
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
                    dto.PurchaseOrderCalloffCode,
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
            [FromBody] List<TagIdWithRowVersionDto> tagDtos)
        {
            var tags = tagDtos.Select(t => new IdAndRowVersion(t.Id, t.RowVersion));
            var result = await _mediator.Send(new TransferCommand(tags));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/CompletePreservation")]
        public async Task<IActionResult> CompletePreservation(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] CompletePreservationDto dto)
        {
            var command = new CompletePreservationCommand(new List<IdAndRowVersion> { new IdAndRowVersion(id, dto.RowVersion) });

            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("CompletePreservation")]
        public async Task<IActionResult> CompletePreservation(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] List<TagIdWithRowVersionDto> tagDtos)
        {
            var tags = tagDtos.Select(t => new IdAndRowVersion(t.Id, t.RowVersion));
            var result = await _mediator.Send(new CompletePreservationCommand(tags));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpPost("{id}/Requirements/{requirementId}/RecordValues")]
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

            var command = new RecordValuesCommand(
                id,
                requirementId,
                numberValues,
                checkBoxValues,
                requirementValuesDto?.Comment);

            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpPost("{id}/Requirements/{requirementId}/Attachment/{fieldId}")]
        public async Task<IActionResult> AddFieldValueAttachment(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementId,
            [FromRoute] int fieldId,
            [FromForm] UploadAttachmentForceOverwriteDto dto)
        {
            await using var stream = dto.File.OpenReadStream();

            var command = new UploadFieldValueAttachmentCommand(
                id,
                requirementId,
                fieldId,
                dto.File.FileName,
                stream);

            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpDelete("{id}/Requirements/{requirementId}/Attachment/{fieldId}")]
        public async Task<IActionResult> DeleteFieldValueAttachment(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementId,
            [FromRoute] int fieldId)
        {
            var command = new DeleteFieldValueAttachmentCommand(
                id,
                requirementId,
                fieldId);

            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpGet("{id}/Requirements/{requirementId}/Attachment/{fieldId}")]
        public async Task<IActionResult> GetFieldValueAttachment(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementId,
            [FromRoute] int fieldId,
            [FromQuery] bool redirect = false)
        {
            var result = await _mediator.Send(new GetFieldValueAttachmentQuery(id, requirementId, fieldId));

            if (result.ResultType != ResultType.Ok)
            {
                return this.FromResult(result);
            }

            if (!redirect)
            {
                return Ok(result.Data.ToString());
            }

            return Redirect(result.Data.ToString());
        }

        [Authorize(Roles = Permissions.PRESERVATION_WRITE)]
        [HttpPost("{id}/Requirements/{requirementId}/Preserve")]
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
            [FromForm] UploadAttachmentWithOverwriteOptionDto dto)
        {
            await using var stream = dto.File.OpenReadStream();

            var command = new UploadTagAttachmentCommand(
                id,
                dto.File.FileName,
                dto.OverwriteIfExists,
                stream);

            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_DETACHFILE)]
        [HttpDelete("{id}/Attachments/{attachmentId}")]
        public async Task<ActionResult<int>> DeleteTagAttachment(
            [FromHeader(Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int attachmentId,
            [FromBody] DeleteTagAttachmentDto dto)
        {
            var command = new DeleteTagAttachmentCommand(
                id,
                attachmentId,
                dto.RowVersion);

            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("{id}/Attachments/{attachmentId}")]
        public async Task<ActionResult> GetTagAttachment(
            [FromHeader(Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int attachmentId,
            [FromQuery] bool redirect = false)
        {
            var result = await _mediator.Send(new GetTagAttachmentQuery(id, attachmentId));

            if (result.ResultType != ResultType.Ok)
            {
                return this.FromResult(result);
            }

            if (!redirect)
            {
                return Ok(result.Data.ToString());
            }

            return Redirect(result.Data.ToString());
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_VOIDUNVOID)]
        [HttpPut("{id}/Void")]
        public async Task<IActionResult> VoidTag(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] VoidTagDto dto)
        {
            var result = await _mediator.Send(new VoidTagCommand(id, dto.RowVersion));

            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_VOIDUNVOID)]
        [HttpPut("{id}/Unvoid")]
        public async Task<IActionResult> UnvoidTag(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] UnvoidTagDto dto)
        {
            var result = await _mediator.Send(new UnvoidTagCommand(id, dto.RowVersion));

            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("{id}/Requirements/{requirementId}/PreservationRecords")]
        public async Task<ActionResult<List<PreservationRecordDto>>> GetPreservationRecords(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int tagRequirementId)
        {
            var result = await _mediator.Send(new GetPreservationRecordsQuery(id, tagRequirementId));
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
