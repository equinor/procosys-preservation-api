using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Common.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceResult;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class GetTagsForExportQueryHandler : GetTagsQueryBase, IRequestHandler<GetTagsForExportQuery, Result<ExportDto>>
    {
        private readonly string _noDataFoundMarker = "null";
        private readonly IReadOnlyContext _context;
        private readonly IPlantProvider _plantProvider;
        private readonly DateTime _utcNow;
        private readonly ILogger<GetTagsForExportQueryHandler> _logger;
        private Domain.Time.Timer _timer;
        private int _maxHistoryExport;

        public GetTagsForExportQueryHandler(
            IReadOnlyContext context, 
            IOptionsSnapshot<TagOptions> options,
            IPlantProvider plantProvider,
            ILogger<GetTagsForExportQueryHandler> logger)
        {
            _context = context;
            _plantProvider = plantProvider;
            _logger = logger;
            _utcNow = TimeService.UtcNow;
            _maxHistoryExport = options.Value.MaxHistoryExport;
        }

        public async Task<Result<ExportDto>> Handle(GetTagsForExportQuery request, CancellationToken cancellationToken)
        {
            _timer = new Domain.Time.Timer();
            _logger.LogInformation("GetTagsForExportQueryHandler start");
            var queryable = CreateQueryableWithFilter(_context, request.ProjectName, request.Filter, _utcNow);

            queryable = AddSorting(request.Sorting, queryable);

            _logger.LogInformation($"GetTagsForExportQueryHandler querying orderedDtos. {_timer.Elapsed()}");
            var orderedDtos = await queryable
                .TagWith("GetTagsForExportQueryHandler: orderedDtos")
                .ToListAsync(cancellationToken);
            _logger.LogInformation($"GetTagsForExportQueryHandler got orderedDtos. {_timer.Elapsed()}");

            var usedFilterDto = await CreateUsedFilterDtoAsync(request.ProjectName, request.Filter);
            if (!orderedDtos.Any())
            {
                return new SuccessResult<ExportDto>(new ExportDto(null, usedFilterDto));
            }

            var tagsIds = orderedDtos.Select(dto => dto.TagId).ToList();
            var getHistory = 
                (request.HistoryExportMode == HistoryExportMode.ExportOne && tagsIds.Count == 1) || // Just to be compatible before client use new endpoint 
                (request.HistoryExportMode == HistoryExportMode.ExportMax && tagsIds.Count <= _maxHistoryExport);

            var journeyIds = orderedDtos.Select(dto => dto.JourneyId).Distinct();

            var tagsWithIncludes = await GetTagsWithIncludesAsync(tagsIds, getHistory, cancellationToken);
            
            var requirementDefinitionIds = tagsWithIncludes.SelectMany(t => t.Requirements)
                .Select(r => r.RequirementDefinitionId).Distinct();
            
            _logger.LogInformation($"GetTagsForExportQueryHandler querying reqDefWithFields. {_timer.Elapsed()}");
            var reqDefWithFields = await (from rd in _context.QuerySet<RequirementDefinition>()
                        .Include(rd => rd.Fields)
                    where requirementDefinitionIds.Contains(rd.Id)
                    select rd)
                .TagWith("GetTagsForExportQueryHandler: reqDefWithFields")
                .ToListAsync(cancellationToken);
            _logger.LogInformation($"GetTagsForExportQueryHandler got reqDefWithFields. {_timer.Elapsed()}");

            // get Journeys with Steps to be able to export journey and step titles
            _logger.LogInformation($"GetTagsForExportQueryHandler querying journeysWithSteps. {_timer.Elapsed()}");
            var journeysWithSteps = await (from j in _context.QuerySet<Journey>()
                        .Include(j => j.Steps)
                    where journeyIds.Contains(j.Id)
                    select j)
                .TagWith("GetTagsForExportQueryHandler: journeysWithSteps")
                .ToListAsync(cancellationToken);
            _logger.LogInformation($"GetTagsForExportQueryHandler got journeysWithSteps. {_timer.Elapsed()}");

            var exportTagDtos = CreateExportTagDtos(
                orderedDtos,
                tagsWithIncludes,
                journeysWithSteps,
                reqDefWithFields);

            if (getHistory)
            {
                await GetHistoryForTagsAsync(tagsIds, exportTagDtos, tagsWithIncludes, reqDefWithFields, cancellationToken);
            }

            _logger.LogInformation("GetTagsForExportQueryHandler end");
            return new SuccessResult<ExportDto>(new ExportDto(exportTagDtos, usedFilterDto));
        }

        private async Task GetHistoryForTagsAsync(
            List<int> tagsIds,
            IList<ExportTagDto> exportTagDtos,
            List<Tag> tagsWithIncludes,
            List<RequirementDefinition> reqDefWithFields,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"GetTagsForExportQueryHandler querying history. {_timer.Elapsed()}");
            var history = await (from h in _context.QuerySet<History>()
                    join tag in _context.QuerySet<Tag>() on h.ObjectGuid equals tag.ObjectGuid
                    join createdBy in _context.QuerySet<Person>() on h.CreatedById equals createdBy.Id
                    where tagsIds.Contains(tag.Id)
                    select new
                    {
                        History = h,
                        TagId = tag.Id,
                        CreatedBy = createdBy
                    })
                .TagWith("GetTagsForExportQueryHandler: history")
                .ToListAsync(cancellationToken);
            _logger.LogInformation($"GetTagsForExportQueryHandler got history. {_timer.Elapsed()}");

            var groupedHistory = history.GroupBy(h => h.TagId);

            foreach (var historyGroup in groupedHistory)
            {
                var singleExportTagDto = exportTagDtos.Single(t => t.TagId == historyGroup.Key);
                var singleTag = tagsWithIncludes.Single(t => t.Id == historyGroup.Key);

                foreach (var dto in historyGroup.OrderByDescending(h => h.History.CreatedAtUtc))
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
                        dto.TagId,
                        dto.History.Description,
                        dto.History.CreatedAtUtc,
                        $"{dto.CreatedBy.FirstName} {dto.CreatedBy.LastName}",
                        dto.History.DueInWeeks,
                        preservationDetails.ToString(),
                        preservationComment));
                }
            }


            _logger.LogInformation($"GetTagsForExportQueryHandler history dto made. {_timer.Elapsed()}");
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
                _logger.LogInformation($"GetTagsForExportQueryHandler querying tagsWithIncludes with details. {_timer.Elapsed()}");
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
                            where tagsIds.Contains(tag.Id)
                            select tag)
                    .TagWith("GetTagsForExportQueryHandler: tagsWithIncludes with details")
                    .ToListAsync(cancellationToken);
                _logger.LogInformation($"GetTagsForExportQueryHandler got tagsWithIncludes with details. {_timer.Elapsed()}");
            }
            else
            {
                // get tags again, including Requirements, Actions and Attachments. See comment in CreateQueryableWithFilter regarding Include and EF
                _logger.LogInformation($"GetTagsForExportQueryHandler querying tagsWithIncludes without details. {_timer.Elapsed()}");
                tagsWithIncludes = await (from tag in _context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
                            .ThenInclude(r => r.PreservationPeriods)
                        .Include(t => t.Attachments)
                        .Include(t => t.Actions)
                            where tagsIds.Contains(tag.Id)
                            select tag)
                    .TagWith("GetTagsForExportQueryHandler: tagsWithIncludes without details")
                    .ToListAsync(cancellationToken);
                _logger.LogInformation($"GetTagsForExportQueryHandler got tagsWithIncludes without details. {_timer.Elapsed()}");
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
        {
            _logger.LogInformation($"GetTagsForExportQueryHandler querying projectDescription. {_timer.Elapsed()}");
            var projectDescription = await (from p in _context.QuerySet<Project>()
                          where p.Name == projectName
                          select p.Description)
                       .TagWith("GetTagsForExportQueryHandler: projectDescription")
                       .SingleOrDefaultAsync();
            _logger.LogInformation($"GetTagsForExportQueryHandler got projectDescription. {_timer.Elapsed()}");
            return projectDescription;
        }

        private async Task<List<string>> GetJourneyTitlesAsync(IList<int> journeyIds)
        {
            if (!journeyIds.Any())
            {
                return new List<string>();
            }

            _logger.LogInformation($"GetTagsForExportQueryHandler querying journeyTitles. {_timer.Elapsed()}");
            var journeyTitles = await (from j in _context.QuerySet<Journey>()
                where journeyIds.Contains(j.Id)
                select j.Title)
                .TagWith("GetTagsForExportQueryHandler: journeyTitles")
                .ToListAsync();
            _logger.LogInformation($"GetTagsForExportQueryHandler got journeyTitles. {_timer.Elapsed()}");
            return journeyTitles;
        }

        private async Task<List<string>> GetModeTitlesAsync(IList<int> modeIds)
        {
            if (!modeIds.Any())
            {
                return new List<string>();
            }

            _logger.LogInformation($"GetTagsForExportQueryHandler querying modeTitles. {_timer.Elapsed()}");
            var modeTitles = await (from m in _context.QuerySet<Mode>()
                where modeIds.Contains(m.Id)
                select m.Title)
                .TagWith("GetTagsForExportQueryHandler: modeTitles")
                .ToListAsync();
            _logger.LogInformation($"GetTagsForExportQueryHandler got modeTitles. {_timer.Elapsed()}");
            return modeTitles;
        }

        private async Task<List<string>> GetResponsibleCodesAsync(IList<int> responsibleIds)
        {
            if (!responsibleIds.Any())
            {
                return new List<string>();
            }

            _logger.LogInformation($"GetTagsForExportQueryHandler querying responsibleCodes. {_timer.Elapsed()}");
            var responsibleCodes = await (from r in _context.QuerySet<Responsible>()
                where responsibleIds.Contains(r.Id)
                select r.Code)
                .TagWith("GetTagsForExportQueryHandler: responsibleCodes")
                .ToListAsync();
            _logger.LogInformation($"GetTagsForExportQueryHandler got responsibleCodes. {_timer.Elapsed()}");
            return responsibleCodes;
        }

        private async Task<List<string>> GetRequirementTypeTitlesAsync(IList<int> requirementTypeIds)
        {
            if (!requirementTypeIds.Any())
            {
                return new List<string>();
            }

            _logger.LogInformation($"GetTagsForExportQueryHandler querying requirementTypeTitles. {_timer.Elapsed()}");
            var requirementTypeTitles = await (from r in _context.QuerySet<RequirementType>()
                where requirementTypeIds.Contains(r.Id)
                select r.Title)
                .TagWith("GetTagsForExportQueryHandler: requirementTypeTitles")
                .ToListAsync();
            _logger.LogInformation($"GetTagsForExportQueryHandler querying requirementTypeTitles. {_timer.Elapsed()}");
            return requirementTypeTitles;
        }

        private IList<ExportTagDto> CreateExportTagDtos(
            List<TagForQueryDto> orderedDtos,
            List<Tag> tagsWithIncludes,
            List<Journey> journeysWithSteps,
            List<RequirementDefinition> reqDefs)
        {
            _logger.LogInformation($"GetTagsForExportQueryHandler create ExportTagDtos. {_timer.Elapsed()}");
            var tags = orderedDtos.Select(dto =>
            {
                var tagWithIncludes = tagsWithIncludes.Single(t => t.Id == dto.TagId);
                var orderedRequirements = tagWithIncludes.OrderedRequirements().ToList();

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
                    dto.TagId,
                    orderedActions.Select(
                        action => new ExportActionDto(
                            action.Id,
                            action.Title,
                            action.Description,
                            action.IsOverDue(),
                            action.DueTimeUtc,
                            action.ClosedAtUtc)).ToList(),
                    orderedRequirements.Select(
                        req => new ExportRequirementDto(
                            req.Id,
                            reqDefs.Single(rd => rd.Id == req.RequirementDefinitionId).Title,
                            req.NextDueTimeUtc,
                            req.GetNextDueInWeeks(),
                            req.HasActivePeriod ? req.ActivePeriod.Comment : null)).ToList(),
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
                    openActionsCount,
                    overdueActionsCount,
                    PurchaseOrderHelper.CreateTitle(dto.PurchaseOrderNo, dto.CalloffNo),
                    dto.Remark,
                    dto.ResponsibleCode,
                    dto.Status.GetDisplayValue(),
                    step.Title,
                    dto.StorageArea,
                    dto.Description,
                    dto.TagNo);
            });

            _logger.LogInformation($"GetTagsForExportQueryHandler ExportTagDtos created. {_timer.Elapsed()}");
            return tags.ToList();
        }
    }
}
