using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Common.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;
using PreservationAction = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;
using Equinor.ProCoSys.Auth.Caches;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant
{
    public class GetTagsCrossPlantQueryHandler : IRequestHandler<GetTagsCrossPlantQuery, Result<List<TagDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IPermissionCache _permissionCache;
        private readonly IPlantSetter _plantSetter;
        private readonly DateTime _utcNow;

        public GetTagsCrossPlantQueryHandler(
            IReadOnlyContext context,
            IPermissionCache permissionCache,
            IPlantSetter plantSetter)
        {
            _context = context;
            _permissionCache = permissionCache;
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
                var plantTitle = await _permissionCache.GetPlantTitleForCurrentUserAsync(project.Plant);
                foreach (var tag in project.Tags.Where(t => !t.IsVoided))
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
    }
}

