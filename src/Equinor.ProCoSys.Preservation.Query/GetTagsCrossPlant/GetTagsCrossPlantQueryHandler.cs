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
            _plantSetter.SetCrossPlantQuery();
            var allProjects = await (from p in _context.QuerySet<Project>()
                    .Include(p => p.Tags)
                        .ThenInclude(t => t.Requirements)
                        .ThenInclude(r => r.PreservationPeriods)
                    .Include(p => p.Tags)
                        .ThenInclude(t => t.Actions)
                    select p)
                .ToListAsync(cancellationToken);

            // get Journeys with Steps to be able to fill journey and step titles
            var allJourneys = await (from j in _context.QuerySet<Journey>()
                        .Include(j => j.Steps)
                    select j)
                .ToListAsync(cancellationToken);

            var allModes = await (from m in _context.QuerySet<Mode>()
                select m).ToListAsync(cancellationToken);

            var allResponsibles = await (from r in _context.QuerySet<Responsible>()
                select r).ToListAsync(cancellationToken);
            
            var allRequirementTypes = await (from rt in _context.QuerySet<RequirementType>()
                        .Include(rt => rt.RequirementDefinitions)
                    select rt)
                .ToListAsync(cancellationToken);
            _plantSetter.ClearCrossPlantQuery();

            var tags = await CreateTagDtosAsync(allProjects, allRequirementTypes);

            FillJourneySpecificData(tags,
                allJourneys,
                allModes,
                allResponsibles);

            var orderedTags = tags
                .OrderBy(a => a.PlantId)
                .ThenBy(a => a.ProjectName)
                .ThenBy(a => a.TagNo)
                .AsEnumerable();

            if (request.Max > 0)
            {
                orderedTags = orderedTags.Take(request.Max);
            }

            return new SuccessResult<List<TagDto>>(orderedTags.ToList());
        }

        private async Task<List<TagDto>> CreateTagDtosAsync(List<Project> projects, List<RequirementType> requirementTypes)
        {
            var tagDtos = new List<TagDto>();
            foreach (var project in projects)
            {
                var plantTitle = await _plantCache.GetPlantTitleAsync(project.Plant);
                foreach (var tag in project.Tags)
                {
                    var requirementDtos = tag.OrderedRequirements().Select(
                            r =>
                            {
                                var requirementType =
                                    requirementTypes.Single(rt =>
                                        rt.RequirementDefinitions.Any(rd => rd.Id == r.RequirementDefinitionId));
                                var requirementDefinition =
                                    requirementType.RequirementDefinitions.Single(rd =>
                                        rd.Id == r.RequirementDefinitionId);
                                
                                return new RequirementDto(
                                    r.Id,
                                    requirementType.Code,
                                    requirementDefinition.Title,
                                    r.NextDueTimeUtc,
                                    r.GetNextDueInWeeks(),
                                    r.IsReadyAndDueToBePreserved());
                            })
                        .ToList();
                    var actionStatus = GetActionStatus(tag.Actions);
                    var tagDto = new TagDto(
                        project.Plant,
                        plantTitle,
                        project.Name,
                        project.Description,
                        project.IsClosed,
                        tag.Id,
                        actionStatus,
                        tag.AreaCode,
                        tag.AreaDescription,
                        tag.Calloff,
                        tag.CommPkgNo,
                        tag.Description,
                        tag.DisciplineCode,
                        tag.DisciplineDescription,
                        tag.IsVoided,
                        tag.McPkgNo,
                        tag.PurchaseOrderNo,
                        tag.IsReadyToBePreserved(),
                        requirementDtos,
                        tag.Status,
                        tag.StepId,
                        tag.TagFunctionCode,
                        tag.TagNo,
                        tag.TagType);
                    tagDtos.Add(tagDto);
                }
            }

            return tagDtos;
        }
        
        private void FillJourneySpecificData(List<TagDto> tags, List<Journey> journeys, List<Mode> modes, List<Responsible> responsibles)
        {
            var ditinctStepIds = tags.Select(t => t.StepId).Distinct();

            foreach (var stepId in ditinctStepIds)
            {
                var journey = journeys.Single(j => j.Steps.Any(s => s.Id == stepId));
                var step = journey.Steps.Single(s => s.Id == stepId);
                var mode = modes.Single(m => m.Id == step.ModeId);
                var resposible = responsibles.Single(r => r.Id == step.ResponsibleId);
                var nextStep = journey.GetNextStep(stepId);
                var nextMode = nextStep != null
                    ? modes.Single(m => m.Id == nextStep.ModeId)
                    : null;
                var nextResponsible = nextStep != null
                    ? responsibles.Single(m => m.Id == nextStep.ResponsibleId)
                    : null;
                tags.Where(t => t.StepId == stepId).ToList()
                    .ForEach(t => t.SetJouyrneyData(
                        mode.Title,
                        resposible.Code,
                        resposible.Description,
                        nextMode?.Title,
                        nextResponsible?.Code,
                        nextResponsible?.Description));
            }
        }

        private ActionStatus? GetActionStatus(IReadOnlyCollection<PreservationAction> actions)
        {
            if (actions.Any(a => !a.ClosedAtUtc.HasValue && a.DueTimeUtc < _utcNow))
            {
                return ActionStatus.HasOverdue;
            }
            if (actions.Any(a => !a.ClosedAtUtc.HasValue))
            {
                return ActionStatus.HasOpen;
            }
            if (actions.Any(a => a.ClosedAtUtc.HasValue))
            {
                return ActionStatus.HasClosed;
            }

            return null;
        }

        //private List<TagDto> CreateTagDtos2(
        //    List<TagInfo> tagInfos,
        //    List<RequirementInfo> reqInfos,
        //    List<Mode> nextModes,
        //    List<Responsible> nextResponsibles)
        //{
        //    var count = 0;
        //    var tagDtos = new List<TagDto>();
        //    foreach (var tagInfo in tagInfos)
        //    {
            
        //        // enrich elements to be able to get distinct NextSteps to query database for distinct NextMode + NextResponsible
        //        foreach (var tag in tags)
        //        {
        //            tagInfo.JourneyWithSteps = allJourneys.Single(j => j.Id == tagInfo.JourneyId);
        //            tagInfo.NextStep = tagInfo.JourneyWithSteps.GetNextStep(tag.StepId);
        //        }

        //        var nextModeIds = tagInfos.Where(dto => dto.NextStep != null).Select(dto => dto.NextStep.ModeId).Distinct();
        //        var nextResponsibleIds = tagInfos.Where(dto => dto.NextStep != null).Select(dto => dto.NextStep.ResponsibleId).Distinct();
        //        var requirementDefinitionIds = tagInfos.SelectMany(t => t.Tag.Requirements).Select(r => r.RequirementDefinitionId).Distinct();

        //        //var isReadyToBePreserved = tagWithIncludes.IsReadyToBePreserved();

        //        //var nextMode = tagWithIncludes.FollowsAJourney && tagInfo.NextStep != null 
        //        //    ? nextModes.Single(m => m.Id == tagInfo.NextStep.ModeId)
        //        //    : null;
                
        //        //var nextResponsible = tagWithIncludes.FollowsAJourney && tagInfo.NextStep != null
        //        //    ? nextResponsibles.Single(m => m.Id == tagInfo.NextStep.ResponsibleId)
        //        //    : null;

        //        Mode nextMode = null;
        //        Responsible nextResponsible = null;
        //        var isReadyToBePreserved = false;
        //        count++;
        //        var tagDto = new TagDto(
        //            tagInfo.PlantId,
        //            tagInfo.PlantTitle,
        //            tagInfo.ProjectName,
        //            tagInfo.ProjectDescription,
        //            tagInfo.IsProjectClosed,
        //            tag.Id,
        //            tagInfo.GetActionStatus(),
        //            tag.AreaCode,
        //            tag.AreaDescription,
        //            tag.Calloff,
        //            tag.CommPkgNo,
        //            tag.Description,
        //            tag.DisciplineCode,
        //            tag.DisciplineDescription,
        //            tag.IsVoided,
        //            tag.McPkgNo,
        //            tagInfo.ModeTitle,
        //            nextMode?.Title,
        //            nextResponsible?.Code,
        //            nextResponsible?.Description,
        //            tag.PurchaseOrderNo,
        //            isReadyToBePreserved,
        //            new List<RequirementDto>(),
        //            tagInfo.ResponsibleCode,
        //            tagInfo.ResponsibleDescription,
        //            tag.Status,
        //            tag.TagFunctionCode,
        //            tag.TagNo,
        //            tag.TagType);
        //        tagDtos.Add(tagDto);
        //    }

        //    return tagDtos.Take(5).ToList();
        //}
    }
}

