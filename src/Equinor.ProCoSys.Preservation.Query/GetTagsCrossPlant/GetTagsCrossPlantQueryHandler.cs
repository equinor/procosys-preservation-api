using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Time;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;
using PreservationAction = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant
{
    public class GetTagsCrossPlantQueryHandler : IRequestHandler<GetTagsCrossPlantQuery, Result<List<TagDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IPlantCache _plantCache;
        private readonly IPlantSetter _plantSetter;
        private readonly DateTime _utcNow;

        public GetTagsCrossPlantQueryHandler(
            IReadOnlyContext context,
            IPlantCache plantCache,
            IPlantSetter plantSetter)
        {
            _context = context;
            _plantCache = plantCache;
            _plantSetter = plantSetter;
            _utcNow = TimeService.UtcNow;
        }

        public async Task<Result<List<TagDto>>> Handle(GetTagsCrossPlantQuery request, CancellationToken cancellationToken)
        {
            var queryable = CreateQueryable(_context, _utcNow);

            _plantSetter.SetCrossPlantQuery();
            var tagElements = await queryable.ToListAsync(cancellationToken);

            var tagsIds = tagElements.Select(dto => dto.TagId).ToList();
            var journeyIds = tagElements.Select(dto => dto.JourneyId).Distinct();

            var tagsWithIncludes = await GetTagsWithIncludesAsync(tagsIds, cancellationToken);

            // get Journeys with Steps to be able to export journey and step titles
            var journeysWithSteps = await (from j in _context.QuerySet<Journey>()
                        .Include(j => j.Steps)
                    where journeyIds.Contains(j.Id)
                    select j)
                .ToListAsync(cancellationToken);
            
            // enrich elements to be able to get distinct NextSteps to query database for distinct NextMode + NextResponsible
            foreach (var te in tagElements)
            {
                te.JourneyWithSteps = journeysWithSteps.Single(j => j.Id == te.JourneyId);
                te.NextStep = te.JourneyWithSteps.GetNextStep(te.StepId);
            }

            var nextModeIds = tagElements.Where(dto => dto.NextStep != null).Select(dto => dto.NextStep.ModeId).Distinct();
            var nextResponsibleIds = tagElements.Where(dto => dto.NextStep != null).Select(dto => dto.NextStep.ResponsibleId).Distinct();
            var requirementDefinitionIds = tagsWithIncludes.SelectMany(t => t.Requirements).Select(r => r.RequirementDefinitionId).Distinct();

            var nextModes = await (from m in _context.QuerySet<Mode>()
                where nextModeIds.Contains(m.Id)
                select m).ToListAsync(cancellationToken);

            var nextResponsibles = await (from r in _context.QuerySet<Responsible>()
                where nextResponsibleIds.Contains(r.Id)
                select r).ToListAsync(cancellationToken);
            
            var reqInfos = await (from rd in _context.QuerySet<RequirementDefinition>()
                    join rt in _context.QuerySet<RequirementType>() on EF.Property<int>(rd, "RequirementTypeId") equals rt.Id
                    where requirementDefinitionIds.Contains(rd.Id)
                    select new RequirementInfo
                    {
                        RequirementDefinitionId = rd.Id,
                        RequirementDefinitionTitle = rd.Title,
                        RequirementTypeCode = rt.Code
                    }
                ).ToListAsync(cancellationToken);
            _plantSetter.ClearCrossPlantQuery();

            await FillPlantTitleAsync(tagElements);

            var tags = CreateTagDtos(
                tagElements,
                tagsWithIncludes,
                reqInfos, 
                nextModes,
                nextResponsibles);

            var orderedTags = tags
                .OrderBy(a => a.PlantId)
                .ThenBy(a => a.ProjectName)
                .ThenBy(a => a.TagNo).ToList();
            return new SuccessResult<List<TagDto>>(orderedTags);
        }

        private List<TagDto> CreateTagDtos(
            List<TagForQuery> tagElements,
            List<Tag> tagsWithIncludes,
            List<RequirementInfo> reqInfos,
            List<Mode> nextModes,
            List<Responsible> nextResponsibles)
        {
            var tags = tagElements.Select(dto =>
            {
                var tagWithIncludes = tagsWithIncludes.Single(t => t.Id == dto.TagId);
                var requirementDtos = tagWithIncludes.OrderedRequirements().Select(
                        r =>
                        {
                            var reqInfo =
                                reqInfos.Single(innerDto =>
                                    innerDto.RequirementDefinitionId == r.RequirementDefinitionId);
                            return new RequirementDto(
                                r.Id,
                                reqInfo.RequirementTypeCode,
                                reqInfo.RequirementDefinitionTitle,
                                r.NextDueTimeUtc,
                                r.GetNextDueInWeeks(),
                                r.IsReadyAndDueToBePreserved());
                        })
                    .ToList();

                var isReadyToBePreserved = tagWithIncludes.IsReadyToBePreserved();

                var nextMode = tagWithIncludes.FollowsAJourney && dto.NextStep != null 
                    ? nextModes.Single(m => m.Id == dto.NextStep.ModeId)
                    : null;
                
                var nextResponsible = tagWithIncludes.FollowsAJourney && dto.NextStep != null
                    ? nextResponsibles.Single(m => m.Id == dto.NextStep.ResponsibleId)
                    : null;

                return new TagDto(
                    dto.PlantId,
                    dto.PlantTitle,
                    dto.ProjectName,
                    dto.ProjectDescription,
                    dto.IsProjectClosed,
                    dto.TagId,
                    dto.GetActionStatus(),
                    dto.AreaCode,
                    dto.AreaDescription,
                    dto.CalloffNo,
                    dto.CommPkgNo,
                    dto.Description,
                    dto.DisciplineCode,
                    dto.DisciplineDescription,
                    dto.IsVoided,
                    dto.McPkgNo,
                    dto.ModeTitle,
                    nextMode?.Title,
                    nextResponsible?.Code,
                    nextResponsible?.Description,
                    dto.PurchaseOrderNo,
                    isReadyToBePreserved,
                    requirementDtos,
                    dto.ResponsibleCode,
                    dto.ResponsibleDescription,
                    dto.Status,
                    dto.TagFunctionCode,
                    dto.TagNo,
                    dto.TagType);
            });

            return tags.ToList();
        }

        private async Task<List<Tag>> GetTagsWithIncludesAsync(List<int> tagsIds, CancellationToken cancellationToken)
            // get tags again, including Requirements, Actions and Attachments. See comment in CreateQueryableWithFilter regarding Include and EF
            => await (from tag in _context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
                        .ThenInclude(r => r.PreservationPeriods)
                    where tagsIds.Contains(tag.Id)
                    select tag)
                .ToListAsync(cancellationToken);

        private async Task FillPlantTitleAsync(List<TagForQuery> tags)
        {
            var plantIds = tags.Select(dto => dto.PlantId).Distinct();
            foreach (var plantId in plantIds)
            {
                var plantTitle = await _plantCache.GetPlantTitleAsync(plantId);
                tags.Where(t => t.PlantId == plantId).ToList()
                    .ForEach(t => t.PlantTitle = plantTitle);
            }
        }

        private IQueryable<TagForQuery> CreateQueryable(IReadOnlyContext context, DateTime utcNow)
        {
            // No .Include() here. EF do not support .Include together with selecting a projection (dto).
            // If the select-statement select tag so queryable has been of type IQueryable<Tag>, .Include(t => t.Requirements) work fine
            var queryable = from tag in context.QuerySet<Tag>()
                join project in context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals project.Id
                join step in context.QuerySet<Step>() on tag.StepId equals step.Id
                join journey in context.QuerySet<Journey>() on EF.Property<int>(step, "JourneyId") equals journey.Id
                join mode in context.QuerySet<Mode>() on step.ModeId equals mode.Id
                join responsible in context.QuerySet<Responsible>() on step.ResponsibleId equals responsible.Id
                let anyOverdueActions = (from a in context.QuerySet<PreservationAction>()
                    where EF.Property<int>(a, "TagId") == tag.Id && !a.ClosedAtUtc.HasValue && a.DueTimeUtc < utcNow
                    select a.Id).Any()
                let anyOpenActions = (from a in context.QuerySet<PreservationAction>()
                    where EF.Property<int>(a, "TagId") == tag.Id && !a.ClosedAtUtc.HasValue
                    select a.Id).Any()
                let anyClosedActions = (from a in context.QuerySet<PreservationAction>()
                    where EF.Property<int>(a, "TagId") == tag.Id && a.ClosedAtUtc.HasValue
                    select a.Id).Any()
                select new TagForQuery
                {
                    AreaCode = tag.AreaCode,
                    AreaDescription = tag.AreaDescription,
                    AnyOverdueActions = anyOverdueActions,
                    AnyOpenActions = anyOpenActions,
                    AnyClosedActions = anyClosedActions,
                    CalloffNo = tag.Calloff,
                    CommPkgNo = tag.CommPkgNo,
                    Description = tag.Description,
                    DisciplineCode = tag.DisciplineCode,
                    DisciplineDescription = tag.DisciplineDescription,
                    IsProjectClosed = project.IsClosed,
                    IsVoided = tag.IsVoided,
                    JourneyId = journey.Id,
                    ModeTitle = mode.Title,
                    McPkgNo = tag.McPkgNo,
                    PlantId = tag.Plant,
                    ProjectName = project.Name,
                    ProjectDescription = project.Description,
                    PurchaseOrderNo = tag.PurchaseOrderNo,
                    ResponsibleCode = responsible.Code,
                    ResponsibleDescription = responsible.Description,
                    Status = tag.Status,
                    StepId = step.Id,
                    TagFunctionCode = tag.TagFunctionCode,
                    TagId = tag.Id,
                    TagNo = tag.TagNo,
                    TagType = tag.TagType
                };
            return queryable;
        }

        private class TagForQuery
        {
            public string AreaCode { get; init; }
            public string AreaDescription { get; init; }
            public bool AnyOverdueActions { get; init; }
            public bool AnyOpenActions { get; init; }
            public bool AnyClosedActions { get; init; }
            public string CalloffNo { get; init; }
            public string CommPkgNo { get; init; }
            public string Description { get; init; }
            public string DisciplineCode { get; init; }
            public string DisciplineDescription { get; init; }
            public bool IsProjectClosed { get; init; }
            public bool IsVoided { get; init; }
            public int JourneyId { get; init; }
            public string McPkgNo { get; init; }
            public string ModeTitle { get; init; }
            public string PlantId { get; init; }
            public string ProjectName { get; init; }
            public string ProjectDescription { get; init; }
            public string PurchaseOrderNo { get; init; }
            public string ResponsibleCode { get; init; }
            public string ResponsibleDescription { get; init; }
            public PreservationStatus Status { get; init; }
            public int StepId { get; init; }
            public int TagId { get; init; }
            public string TagFunctionCode { get; init; }
            public string TagNo { get; init; }
            public TagType TagType { get; init; }

            public string PlantTitle { get; set; }
            public Journey JourneyWithSteps { get; set; }
            public Step NextStep { get; set; }

            public ActionStatus? GetActionStatus()
            {
                if (AnyOverdueActions)
                {
                    return ActionStatus.HasOverdue;
                }
                if (AnyOpenActions)
                {
                    return ActionStatus.HasOpen;
                }
                if (AnyClosedActions)
                {
                    return ActionStatus.HasClosed;
                }

                return null;
            }
        }

        private class RequirementInfo
        {
            public int RequirementDefinitionId { get; set; }
            public string RequirementDefinitionTitle { get; set; }
            public string RequirementTypeCode { get; set; }
        }
    }
}

