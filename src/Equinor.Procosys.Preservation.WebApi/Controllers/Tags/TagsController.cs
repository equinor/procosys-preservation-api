using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Delete;
using Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Upload;
using Equinor.Procosys.Preservation.Command.ActionCommands.CloseAction;
using Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction;
using Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction;
using Equinor.Procosys.Preservation.Command.RequirementCommands.DeleteAttachment;
using Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues;
using Equinor.Procosys.Preservation.Command.RequirementCommands.Upload;
using Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Delete;
using Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload;
using Equinor.Procosys.Preservation.Command.TagCommands.AutoScopeTags;
using Equinor.Procosys.Preservation.Command.TagCommands.AutoTransfer;
using Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve;
using Equinor.Procosys.Preservation.Command.TagCommands.CompletePreservation;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTags;
using Equinor.Procosys.Preservation.Command.TagCommands.DeleteTag;
using Equinor.Procosys.Preservation.Command.TagCommands.DuplicateAreaTag;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Command.TagCommands.Reschedule;
using Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using Equinor.Procosys.Preservation.Command.TagCommands.UnvoidTag;
using Equinor.Procosys.Preservation.Command.TagCommands.UpdateTag;
using Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements;
using Equinor.Procosys.Preservation.Command.TagCommands.VoidTag;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.CheckAreaTagNo;
using Equinor.Procosys.Preservation.Query.GetActionAttachment;
using Equinor.Procosys.Preservation.Query.GetActionAttachments;
using Equinor.Procosys.Preservation.Query.GetActionDetails;
using Equinor.Procosys.Preservation.Query.GetActions;
using Equinor.Procosys.Preservation.Query.GetFieldValueAttachment;
using Equinor.Procosys.Preservation.Query.GetHistoricalFieldValueAttachment;
using Equinor.Procosys.Preservation.Query.GetHistory;
using Equinor.Procosys.Preservation.Query.GetPreservationRecord;
using Equinor.Procosys.Preservation.Query.GetTagAttachment;
using Equinor.Procosys.Preservation.Query.GetTagAttachments;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Equinor.Procosys.Preservation.Query.GetTagRequirements;
using Equinor.Procosys.Preservation.Query.GetTagsQueries;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTags;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;
using Equinor.Procosys.Preservation.WebApi.Excel;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult;
using ServiceResult.ApiExtensions;
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
        private readonly IExcelConverter _excelConverter;

        public TagsController(IMediator mediator, IExcelConverter excelConverter)
        {
            _mediator = mediator;
            _excelConverter = excelConverter;
        }

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet]
        public async Task<ActionResult<TagsResult>> GetTags(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] FilterDto filter,
            [FromQuery] SortingDto sorting,
            [FromQuery] PagingDto paging)
        {
            var query = CreateGetTagsQuery(filter, sorting, paging);

            var result = await _mediator.Send(query);
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("ExportTagsToExcel")]
        public async Task<ActionResult> ExportTagsToExcel(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] FilterDto filter,
            [FromQuery] SortingDto sorting)
        {
            var query = CreateGetTagsForExportQuery(filter, sorting);

            var result = await _mediator.Send(query);

            if (result.ResultType != ResultType.Ok)
            {
                return this.FromResult(result);
            }

            var excelMemoryStream = _excelConverter.Convert(result.Data);
            excelMemoryStream.Position = 0;

            return File(excelMemoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{_excelConverter.GetFileName()}.xlsx");  
        }

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}")]
        public async Task<ActionResult<TagDetailsDto>> GetTagDetails(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagDetailsQuery(id));
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}/Requirements")]
        public async Task<ActionResult<List<RequirementDetailsDto>>> GetTagRequirements(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromQuery] bool includeVoided = false,
            [FromQuery] bool includeAllUsages = false)
        {
            var result = await _mediator.Send(new GetTagRequirementsQuery(id, includeVoided, includeAllUsages));
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}/Actions")]
        public async Task<ActionResult<List<ActionDto>>> GetTagActions(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetActionsQuery(id));
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}/Actions/{actionId}")]
        public async Task<ActionResult<ActionDetailsDto>> GetTagActionDetails(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromRoute] int actionId)
        {
            var result = await _mediator.Send(new GetActionDetailsQuery(id, actionId));
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_CREATE, Permissions.PRESERVATION_PLAN_CREATE)]
        [HttpPost("{id}/Actions")]
        public async Task<ActionResult<int>> CreateAction(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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

        [AuthorizeAny(Permissions.PRESERVATION_WRITE, Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/Actions/{actionId}")]
        public async Task<ActionResult<string>> UpdateAction(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
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
         
        [AuthorizeAny(Permissions.PRESERVATION_WRITE, Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/Actions/{actionId}/Close")]
        public async Task<ActionResult<string>> CloseAction(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
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
       
        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}/Actions/{actionId}/Attachments")]
        public async Task<ActionResult<List<ActionAttachmentDto>>> GetActionAttachments(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromRoute] int actionId)
        {
            var result = await _mediator.Send(new GetActionAttachmentsQuery(id, actionId));
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}/Actions/{actionId}/Attachments/{attachmentId}")]
        public async Task<ActionResult> GetActionAttachment(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
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

        [AuthorizeAny(Permissions.PRESERVATION_ATTACHFILE, Permissions.PRESERVATION_PLAN_ATTACHFILE)]
        [HttpPost("{id}/Actions/{actionId}/Attachments")]
        public async Task<ActionResult<int>> UploadActionAttachment(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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

        [AuthorizeAny(Permissions.PRESERVATION_DETACHFILE, Permissions.PRESERVATION_PLAN_DETACHFILE)]
        [HttpDelete("{id}/Actions/{actionId}/Attachments/{attachmentId}")]
        public async Task<ActionResult<int>> DeleteActionAttachment(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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

        [AuthorizeAny(Permissions.PRESERVATION_WRITE, Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateTag(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
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

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/UpdateTagStepAndRequirements")]
        public async Task<ActionResult<string>> UpdateTagStepAndRequirements(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] UpdateTagStepAndRequirementsDto dto)
        {
            var newRequirements = dto.NewRequirements?.
                Select(r => new RequirementForCommand(r.RequirementDefinitionId, r.IntervalWeeks)).ToList();

            var updatedRequirements = dto.UpdatedRequirements?.Select(r =>
                new UpdateRequirementForCommand(r.RequirementId, r.IntervalWeeks, r.IsVoided, r.RowVersion)).ToList();

            var command = new UpdateTagStepAndRequirementsCommand(id, dto.Description, dto.StepId, updatedRequirements, newRequirements, dto.RowVersion);

            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_CREATE)]
        [HttpPost("Standard")]
        public async Task<ActionResult<int>> CreateTags(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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
        [HttpPost("MigrateStandard")]
        public async Task<ActionResult<int>> MigrateTags(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] AutoScopeTagsDto dto)
        {
            var result = await _mediator.Send(
                new AutoScopeTagsCommand(
                    dto.TagNos.ToList(),
                    dto.ProjectName,
                    dto.StepId,
                    dto.Remark,
                    dto.StorageArea));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_CREATE)]
        [HttpPost("Area")]
        public async Task<ActionResult<int>> CreateAreaTag(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_CREATE)]
        [HttpPost("DuplicateArea")]
        public async Task<ActionResult<int>> DuplicateAreaTag(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] DuplicateAreaTagDto dto)
        {
            var command = new DuplicateAreaTagCommand(
                dto.SourceTagId,
                dto.AreaTagType.ConvertToTagType(),
                dto.DisciplineCode,
                dto.AreaCode,
                dto.TagNoSuffix,
                dto.Description,
                dto.Remark,
                dto.StorageArea);

            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("CheckAreaTagNo")]
        public async Task<ActionResult<Query.CheckAreaTagNo.AreaTagDto>> CheckAreaTagNo(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
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
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new StartPreservationCommand(new List<int>{id}));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("StartPreservation")]
        public async Task<IActionResult> StartPreservation(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new StartPreservationCommand(tagIds));
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_WRITE, Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/Preserve")]
        public async Task<IActionResult> PreserveTag(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new PreserveCommand(id));
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_WRITE, Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("BulkPreserve")]
        public async Task<IActionResult> BulkPreserveTags(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new BulkPreserveCommand(tagIds));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("Transfer")]
        public async Task<ActionResult<List<IdAndRowVersion>>> Transfer(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromBody] List<TagIdWithRowVersionDto> tagDtos)
        {
            var tags = tagDtos?.Select(t => new IdAndRowVersion(t.Id, t.RowVersion));
            var result = await _mediator.Send(new TransferCommand(tags));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("AutoTransfer")]
        public async Task<IActionResult> AutoTransfer(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromBody] AutoTransferDto dto)
        {
            var result = await _mediator.Send(new AutoTransferCommand(dto.ProjectName, dto.CertificateNo, dto.CertificateType));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("{id}/CompletePreservation")]
        public async Task<IActionResult> CompletePreservation(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
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
        public async Task<ActionResult<List<IdAndRowVersion>>> CompletePreservation(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromBody] List<TagIdWithRowVersionDto> tagDtos)
        {
            var tags = tagDtos?.Select(t => new IdAndRowVersion(t.Id, t.RowVersion));
            var result = await _mediator.Send(new CompletePreservationCommand(tags));
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_WRITE, Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPost("{id}/Requirements/{requirementId}/RecordValues")]
        public async Task<IActionResult> RecordValues(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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

        [AuthorizeAny(Permissions.PRESERVATION_WRITE, Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPost("{id}/Requirements/{requirementId}/Attachment/{fieldId}")]
        public async Task<IActionResult> UploadFieldValueAttachment(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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

        [AuthorizeAny(Permissions.PRESERVATION_WRITE, Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpDelete("{id}/Requirements/{requirementId}/Attachment/{fieldId}")]
        public async Task<IActionResult> DeleteFieldValueAttachment(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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

        [AuthorizeAny(Permissions.PRESERVATION_WRITE, Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpGet("{id}/Requirements/{requirementId}/Attachment/{fieldId}")]
        public async Task<IActionResult> GetFieldValueAttachment(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
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

        [AuthorizeAny(Permissions.PRESERVATION_WRITE, Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPost("{id}/Requirements/{requirementId}/Preserve")]
        public async Task<IActionResult> PreserveRequirement
        (
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int requirementId)
        {
            var result = await _mediator.Send(new RequirementPreserveCommand(id, requirementId));
            
            return this.FromResult(result);
        }
        
        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}/Attachments")]
        public async Task<ActionResult<List<TagAttachmentDto>>> GetTagAttachments(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagAttachmentsQuery(id));
            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_ATTACHFILE, Permissions.PRESERVATION_PLAN_ATTACHFILE)]
        [HttpPost("{id}/Attachments")]
        public async Task<ActionResult<int>> UploadTagAttachment(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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

        [AuthorizeAny(Permissions.PRESERVATION_DETACHFILE, Permissions.PRESERVATION_PLAN_DETACHFILE)]
        [HttpDelete("{id}/Attachments/{attachmentId}")]
        public async Task<ActionResult<int>> DeleteTagAttachment(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
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

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}/Attachments/{attachmentId}")]
        public async Task<ActionResult> GetTagAttachment(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
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
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
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
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] UnvoidTagDto dto)
        {
            var result = await _mediator.Send(new UnvoidTagCommand(id, dto.RowVersion));

            return this.FromResult(result);
        }

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}/Requirements/{tagRequirementId}/PreservationRecord/{preservationRecordGuid}")]
        public async Task<ActionResult<PreservationRecordDto>>GetPreservationRecord(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromRoute] int tagRequirementId,
            [FromRoute] Guid preservationRecordGuid)
        {
            var result = await _mediator.Send(new GetPreservationRecordQuery(id, tagRequirementId, preservationRecordGuid));
            return this.FromResult(result);
        }


        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}/Requirements/{tagRequirementId}/PreservationRecord/{preservationRecordGuid}/Attachment")]
        public async Task<IActionResult> GetHistoricalFieldValueAttachment(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromRoute] int tagRequirementId,
            [FromRoute] Guid preservationRecordGuid,
            [FromQuery] bool redirect = false)
        {
            var result = await _mediator.Send(new GetHistoricalFieldValueAttachmentQuery(id, tagRequirementId, preservationRecordGuid));

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

        [AuthorizeAny(Permissions.PRESERVATION_READ, Permissions.PRESERVATION_PLAN_READ)]
        [HttpGet("{id}/History")]
        public async Task<ActionResult<List<HistoryDto>>> GetTagHistory(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetHistoryQuery(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_DELETE)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTag(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] DeleteTagDto dto)
        {
            var result = await _mediator.Send(new DeleteTagCommand(id, dto.RowVersion));
            return this.FromResult(result);
        }
        
        [Authorize(Roles = Permissions.PRESERVATION_PLAN_WRITE)]
        [HttpPut("Reschedule")]
        public async Task<IActionResult> Reschedule (
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromBody] RescheduleTagsDto dto)
        {
            var tags = dto.Tags.Select(t => new IdAndRowVersion(t.Id, t.RowVersion));
            var result = await _mediator.Send(new RescheduleCommand(tags, dto.Weeks, dto.Direction, dto.Comment));
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

            FillFilterFromDto(filter, query.Filter);

            return query;
        }

        private static GetTagsForExportQuery CreateGetTagsForExportQuery(FilterDto filter, SortingDto sorting)
        {
            var query = new GetTagsForExportQuery(
                filter.ProjectName,
                new Sorting(sorting.Direction, sorting.Property),
                new Filter()
            );

            FillFilterFromDto(filter, query.Filter);

            return query;
        }

        private static void FillFilterFromDto(FilterDto source, Filter target)
        {
            if (source.VoidedFilter.HasValue)
            {
                target.VoidedFilter = source.VoidedFilter.Value;
            }

            if (source.ActionStatus.HasValue)
            {
                target.ActionStatus = source.ActionStatus;
            }

            if (source.DueFilters != null)
            {
                target.DueFilters = source.DueFilters.ToList();
            }

            if (source.PreservationStatus.HasValue)
            {
                target.PreservationStatus = source.PreservationStatus;
            }

            if (source.RequirementTypeIds != null)
            {
                target.RequirementTypeIds = source.RequirementTypeIds.ToList();
            }

            if (source.AreaCodes != null)
            {
                target.AreaCodes = source.AreaCodes.ToList();
            }

            if (source.DisciplineCodes != null)
            {
                target.DisciplineCodes = source.DisciplineCodes.ToList();
            }

            if (source.ResponsibleIds != null)
            {
                target.ResponsibleIds = source.ResponsibleIds.ToList();
            }

            if (source.TagFunctionCodes != null)
            {
                target.TagFunctionCodes = source.TagFunctionCodes.ToList();
            }

            if (source.ModeIds != null)
            {
                target.ModeIds = source.ModeIds.ToList();
            }

            if (source.JourneyIds != null)
            {
                target.JourneyIds = source.JourneyIds.ToList();
            }

            if (source.StepIds != null)
            {
                target.StepIds = source.StepIds.ToList();
            }

            if (source.TagNoStartsWith != null)
            {
                target.TagNoStartsWith = source.TagNoStartsWith;
            }

            if (source.CommPkgNoStartsWith != null)
            {
                target.CommPkgNoStartsWith = source.CommPkgNoStartsWith;
            }

            if (source.McPkgNoStartsWith != null)
            {
                target.McPkgNoStartsWith = source.McPkgNoStartsWith;
            }

            if (source.PurchaseOrderNoStartsWith != null)
            {
                target.PurchaseOrderNoStartsWith = source.PurchaseOrderNoStartsWith;
            }

            if (source.StorageAreaStartsWith != null)
            {
                target.StorageAreaStartsWith = source.StorageAreaStartsWith;
            }

            if (source.CallOffStartsWith != null)
            {
                target.CallOffStartsWith = source.CallOffStartsWith;
            }
        }
    }
}
