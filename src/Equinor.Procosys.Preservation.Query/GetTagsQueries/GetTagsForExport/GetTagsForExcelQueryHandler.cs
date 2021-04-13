using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class GetTagsForExportQueryHandler : GetTagsQueryBase, IRequestHandler<GetTagsForExportQuery, Result<ExportDto>>
    {
        private readonly string _noDataFoundMarker = "null";
        private readonly IReadOnlyContext _context;
        private readonly IPlantProvider _plantProvider;
        private readonly DateTime _utcNow;

        public GetTagsForExportQueryHandler(IReadOnlyContext context, IPlantProvider plantProvider)
        {
            _context = context;
            _plantProvider = plantProvider;
            _utcNow = TimeService.UtcNow;
        }

        public async Task<Result<ExportDto>> Handle(GetTagsForExportQuery request, CancellationToken cancellationToken)
        {
            var queryable = CreateQueryableWithFilter(_context, request.ProjectName, request.Filter, _utcNow);

            queryable = AddSorting(request.Sorting, queryable);

            var orderedDtos = await queryable.ToListAsync(cancellationToken);

            var usedFilterDto = await CreateUsedFilterDtoAsync(request.ProjectName, request.Filter);
            if (!orderedDtos.Any())
            {
                return new SuccessResult<ExportDto>(new ExportDto(null, usedFilterDto));
            }

            var tagsIds = orderedDtos.Select(dto => dto.TagId).ToList();
            var getHistory = tagsIds.Count == 1;
            var journeyIds = orderedDtos.Select(dto => dto.JourneyId).Distinct();

            var tagsWithIncludes = await GetTagsWithIncludesAsync(tagsIds, getHistory, cancellationToken);
            
            var requirementDefinitionIds = tagsWithIncludes.SelectMany(t => t.Requirements)
                .Select(r => r.RequirementDefinitionId).Distinct();
            
            var reqDefWithFields = await (from rd in _context.QuerySet<RequirementDefinition>().Include(rd => rd.Fields)
                    where requirementDefinitionIds.Contains(rd.Id)
                    select rd).ToListAsync(cancellationToken);

            // get Journeys with Steps to be able to export journey and step titles
            var journeysWithSteps = await (from j in _context.QuerySet<Journey>()
                        .Include(j => j.Steps)
                    where journeyIds.Contains(j.Id)
                    select j)
                .ToListAsync(cancellationToken);

            var exportTagDtos = CreateExportTagDtos(
                orderedDtos,
                tagsWithIncludes,
                journeysWithSteps,
                reqDefWithFields);

            if (getHistory)
            {
                await GetHistoryForSingleTagAsync(tagsIds.Single(), exportTagDtos, tagsWithIncludes, reqDefWithFields, cancellationToken);
            }

            return new SuccessResult<ExportDto>(new ExportDto(exportTagDtos, usedFilterDto));
        }

        private async Task GetHistoryForSingleTagAsync(
            int singleTagId,
            IList<ExportTagDto> exportTagDtos,
            List<Tag> tagsWithIncludes,
            List<RequirementDefinition> reqDefWithFields,
            CancellationToken cancellationToken)
        {
            var history = await (from h in _context.QuerySet<History>()
                    join tag in _context.QuerySet<Tag>() on h.ObjectGuid equals tag.ObjectGuid
                    join createdBy in _context.QuerySet<Person>() on h.CreatedById equals createdBy.Id
                    where tag.Id == singleTagId
                    select new
                    {
                        History = h,
                        CreatedBy = createdBy
                    })
                .ToListAsync(cancellationToken);

            var singleExportTagDto = exportTagDtos.Single();
            var singleTag = tagsWithIncludes.Single();

            foreach (var dto in history.OrderByDescending(x => x.History.CreatedAtUtc))
            {
                var preservationComment = string.Empty;
                var preservationDetails = new StringBuilder();

                if (dto.History.PreservationRecordGuid.HasValue)
                {
                    (preservationComment, preservationDetails) = GetPreservationDetailsFromPeriod(
                        dto.History.PreservationRecordGuid.Value,
                        singleTag,
                        reqDefWithFields);
                }

                singleExportTagDto.History.Add(new ExportHistoryDto(
                    dto.History.Id,
                    dto.History.Description,
                    dto.History.CreatedAtUtc,
                    $"{dto.CreatedBy.FirstName} {dto.CreatedBy.LastName}",
                    dto.History.DueInWeeks,
                    preservationDetails.ToString(),
                    preservationComment));
            }
        }

        private (string, StringBuilder) GetPreservationDetailsFromPeriod(
            Guid preservationRecordGuid,
            Tag singleTag,
            List<RequirementDefinition> reqDefWithFields)
        {
            var tagRequirement =
                singleTag.Requirements
                    .Single(r =>
                        r.PreservationPeriods.Any(pp =>
                            pp.PreservationRecord != null &&
                            pp.PreservationRecord.ObjectGuid == preservationRecordGuid));

            var preservationPeriod = tagRequirement
                .PreservationPeriods
                .Single(pp =>
                    pp.PreservationRecord != null &&
                    pp.PreservationRecord.ObjectGuid == preservationRecordGuid);

            var reqDefWithField = reqDefWithFields.Single(r => r.Id == tagRequirement.RequirementDefinitionId);

            var preservationDetails = new StringBuilder();
            foreach (var field in reqDefWithField.Fields)
            {
                GetPreservationDetailsFromField(preservationDetails, field, preservationPeriod);
            }

            return (preservationPeriod.Comment, preservationDetails);
        }

        private void GetPreservationDetailsFromField(
            StringBuilder preservationDetails, Field field,
            PreservationPeriod preservationPeriod)
        {
            if (!field.FieldType.NeedsUserInput())
            {
                return;
            }

            preservationDetails.Append($"{field.Label}=");

            var currentValue = preservationPeriod.GetFieldValue(field.Id);
            var value = string.Empty;
            switch (field.FieldType)
            {
                case FieldType.Number:
                    value = GetNumberValueAsString(currentValue);
                    break;
                case FieldType.CheckBox:
                    value = GetCheckBoxValueAsString(currentValue);
                    break;
                case FieldType.Attachment:
                    value = GetAttachmentValueAsString(currentValue);
                    break;
            }
            preservationDetails.Append($"{value}. ");
        }

        private string GetAttachmentValueAsString(FieldValue fieldValue)
        {
            if (!(fieldValue is AttachmentValue))
            {
                return _noDataFoundMarker;
            }
            var av = (AttachmentValue) fieldValue;
            return av.FieldValueAttachment.FileName;

        }

        private string GetCheckBoxValueAsString(FieldValue fieldValue)
        {
            if (!(fieldValue is CheckBoxChecked))
            {
                return _noDataFoundMarker;
            }
            // A CheckBox checked is true if fieldValue is of type CheckBoxChecked
            return "true";
        }

        private string GetNumberValueAsString(FieldValue fieldValue)
        {
            if (!(fieldValue is NumberValue))
            {
                return _noDataFoundMarker;
            }
            var number = (NumberValue) fieldValue;
            return number.Value.HasValue ? number.Value.ToString() : "N/A";
        }

        private async Task<List<Tag>> GetTagsWithIncludesAsync(List<int> tagsIds, bool getHistory, CancellationToken cancellationToken)
        {
            List<Tag> tagsWithIncludes;
            if (getHistory)
            {
                // get tags again, including Requirements, Actions, Attachments and preservation details. See comment in CreateQueryableWithFilter regarding Include and EF
                // Preservation details found to enrich History info when one tag found
                tagsWithIncludes = await (from tag in _context.QuerySet<Tag>()
                            .Include(t => t.Requirements)
                            .ThenInclude(r => r.PreservationPeriods)
                            .ThenInclude(p => p.PreservationRecord)
                            .Include(t => t.Requirements)
                            .ThenInclude(r => r.PreservationPeriods)
                            .ThenInclude(p => p.FieldValues)
                            .ThenInclude(fv => fv.FieldValueAttachment)
                            .Include(t => t.Attachments)
                            .Include(t => t.Actions)
                        where tag.Id == tagsIds.Single()
                        select tag)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                // get tags again, including Requirements, Actions and Attachments. See comment in CreateQueryableWithFilter regarding Include and EF
                tagsWithIncludes = await (from tag in _context.QuerySet<Tag>()
                            .Include(t => t.Requirements)
                            .Include(t => t.Attachments)
                            .Include(t => t.Actions)
                        where tagsIds.Contains(tag.Id)
                        select tag)
                    .ToListAsync(cancellationToken);
            }

            return tagsWithIncludes;
        }

        private async Task<UsedFilterDto> CreateUsedFilterDtoAsync(string projectName, Filter filter)
        {
            var projectDescription = await GetProjectDescriptionAsync(projectName);
            var requirementTypeTitles = await GetRequirementTypeTitlesAsync(filter.RequirementTypeIds);
            var responsibleCodes = await GetResponsibleCodesAsync(filter.ResponsibleIds);
            var modeTitles = await GetModeTitlesAsync(filter.ModeIds);
            var journeyTitles = await GetJourneyTitlesAsync(filter.JourneyIds);

            return new UsedFilterDto(
                filter.ActionStatus.GetDisplayValue(),
                filter.AreaCodes,
                filter.CallOffStartsWith,
                filter.CommPkgNoStartsWith,
                filter.DisciplineCodes,
                filter.DueFilters.Select(v => v.GetDisplayValue()), 
                journeyTitles,
                filter.McPkgNoStartsWith,
                modeTitles,
                filter.PreservationStatus.GetDisplayValue(),
                projectDescription,
                _plantProvider.Plant,
                projectName,
                filter.PurchaseOrderNoStartsWith,
                requirementTypeTitles,
                responsibleCodes,
                filter.StorageAreaStartsWith,
                filter.TagFunctionCodes,
                filter.TagNoStartsWith,
                filter.VoidedFilter.GetDisplayValue());
        }

        private async Task<string> GetProjectDescriptionAsync(string projectName) 
            => await (from p in _context.QuerySet<Project>()
                where p.Name == projectName
                select p.Description).SingleOrDefaultAsync();

        private async Task<List<string>> GetJourneyTitlesAsync(IList<int> journeyIds)
        {
            if (!journeyIds.Any())
            {
                return new List<string>();
            }

            return await (from j in _context.QuerySet<Journey>()
                where journeyIds.Contains(j.Id)
                select j.Title).ToListAsync();
        }

        private async Task<List<string>> GetModeTitlesAsync(IList<int> modeIds)
        {
            if (!modeIds.Any())
            {
                return new List<string>();
            }

            return await (from m in _context.QuerySet<Mode>()
                where modeIds.Contains(m.Id)
                select m.Title).ToListAsync();
        }

        private async Task<List<string>> GetResponsibleCodesAsync(IList<int> responsibleIds)
        {
            if (!responsibleIds.Any())
            {
                return new List<string>();
            }

            return await (from r in _context.QuerySet<Responsible>()
                where responsibleIds.Contains(r.Id)
                select r.Code).ToListAsync();
        }

        private async Task<List<string>> GetRequirementTypeTitlesAsync(IList<int> requirementTypeIds)
        {
            if (!requirementTypeIds.Any())
            {
                return new List<string>();
            }

            return await (from r in _context.QuerySet<RequirementType>()
                where requirementTypeIds.Contains(r.Id)
                select r.Title).ToListAsync();
        }

        private IList<ExportTagDto> CreateExportTagDtos(
            List<TagForQueryDto> orderedDtos,
            List<Tag> tagsWithIncludes,
            List<Journey> journeysWithSteps,
            List<RequirementDefinition> reqDefs)
        {
            var tags = orderedDtos.Select(dto =>
            {
                var tagWithIncludes = tagsWithIncludes.Single(t => t.Id == dto.TagId);
                var orderedRequirements = tagWithIncludes.OrderedRequirements().ToList();
                var requirementTitles = orderedRequirements
                    .Select(r => reqDefs.Single(rd => rd.Id == r.RequirementDefinitionId).Title)
                    .ToList();

                int? nextDueWeeks = null;
                var nextDueAsYearAndWeek = string.Empty;

                var firstUpcomingRequirement = orderedRequirements.FirstOrDefault();
                if (firstUpcomingRequirement != null)
                {
                    nextDueWeeks = firstUpcomingRequirement.GetNextDueInWeeks();
                    nextDueAsYearAndWeek = firstUpcomingRequirement.NextDueTimeUtc?.FormatAsYearAndWeekString();
                }

                var journeyWithSteps = journeysWithSteps.Single(j => j.Id == dto.JourneyId);
                var step = journeyWithSteps.Steps.Single(s => s.Id == dto.StepId);

                var openActionsCount = tagWithIncludes.Actions.Count(a => !a.IsClosed);
                var overdueActionsCount = tagWithIncludes.Actions.Count(a => a.IsOverDue());

                var orderedActions = tagWithIncludes
                    .Actions
                    .OrderBy(t => t.IsClosed.Equals(true))
                    .ThenByDescending(t => t.DueTimeUtc.HasValue)
                    .ThenBy(t => t.DueTimeUtc)
                    .ThenBy(t => t.ModifiedAtUtc)
                    .ThenBy(t => t.CreatedAtUtc);

                return new ExportTagDto(
                    orderedActions.Select(
                        action => new ExportActionDto(
                            action.Id,
                            action.Title,
                            action.Description,
                            action.IsOverDue(),
                            action.DueTimeUtc,
                            action.ClosedAtUtc)).ToList(),
                    dto.GetActionStatus().GetDisplayValue(),
                    tagWithIncludes.Actions.Count,
                    dto.AreaCode,
                    tagWithIncludes.Attachments.Count,
                    dto.CommPkgNo,
                    dto.DisciplineCode,
                    dto.IsVoided,
                    journeyWithSteps.Title,
                    dto.McPkgNo,
                    dto.ModeTitle,
                    nextDueAsYearAndWeek,
                    nextDueWeeks,
                    openActionsCount,
                    overdueActionsCount,
                    PurchaseOrderHelper.CreateTitle(dto.PurchaseOrderNo, dto.CalloffNo),
                    dto.Remark,
                    string.Join(",", requirementTitles),
                    dto.ResponsibleCode,
                    dto.Status.GetDisplayValue(),
                    step.Title,
                    dto.StorageArea,
                    dto.Description,
                    dto.TagNo);
            });

            return tags.ToList();
        }
    }
}
