using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class GetAllTagsInProjectQueryHandler : IRequestHandler<GetAllTagsInProjectQuery, Result<IEnumerable<TagDto>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IModeRepository _modeRepository;
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly ITimeService _timeService;

        public GetAllTagsInProjectQueryHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IModeRepository modeRepository,
            IResponsibleRepository responsibleRepository,
            ITimeService timeService)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _modeRepository = modeRepository;
            _responsibleRepository = responsibleRepository;
            _timeService = timeService;
        }

        public async Task<Result<IEnumerable<TagDto>>> Handle(GetAllTagsInProjectQuery request, CancellationToken cancellationToken)
        {
            var unOrderedTags = await _projectRepository.GetAllTagsInProjectAsync(request.ProjectName);

            if (!unOrderedTags.Any())
            {
                return new SuccessResult<IEnumerable<TagDto>>(new List<TagDto>());
            }

            var tags = unOrderedTags
                .OrderByDescending(t => t.NextDueTimeUtc.HasValue)
                .ThenBy(t => t.NextDueTimeUtc)
                .ThenBy(t => t.Status)
                .ThenBy(t => t.Id)
                .ToList();

            var stepIds = tags.Select(t => t.StepId).Distinct().ToList();
            var journeys = await _journeyRepository.GetJourneysByStepIdsAsync(stepIds);

            var steps = journeys.SelectMany(s => s.Steps).Where(s => stepIds.Contains(s.Id)).ToList();
            var modeIds = steps.Select(s => s.ModeId).Distinct();
            var modes = await _modeRepository.GetByIdsAsync(modeIds);

            var respIds = steps.Select(s => s.ResponsibleId).Distinct();
            var responsibles = await _responsibleRepository.GetByIdsAsync(respIds);

            var now = _timeService.GetCurrentTimeUtc();

            var tagDtos = tags.Select(tag =>
            {
                var requirementDtos = tag.OrderedRequirements().Select(
                        r => new RequirementDto(
                            r.Id,
                            r.RequirementDefinitionId,
                            r.NextDueTimeUtc,
                            r.GetNextDueInWeeks(now),
                            r.ReadyToBePreserved,
                            r.IsReadyAndDueToBePreserved(now)))
                    .ToList();

                var journey = journeys.Single(j => j.Steps.Any(s => s.Id == tag.StepId));
                var step = steps.Single(s => s.Id == tag.StepId);
                var mode = modes.Single(m => m.Id == step.ModeId);
                var resp = responsibles.Single(r => r.Id == step.ResponsibleId);

                return new TagDto(tag.Id,
                    tag.AreaCode,
                    tag.Calloff,
                    tag.CommPkgNo,
                    tag.DisciplineCode,
                    tag.IsVoided,
                    tag.McPkgNo,
                    mode.Title,
                    tag.IsReadyToBePreserved(now),
                    tag.IsReadyToBeTransferred(journey),
                    tag.PurchaseOrderNo,
                    tag.Remark,
                    requirementDtos,
                    resp.Code,
                    tag.Status,
                    tag.TagFunctionCode,
                    tag.Description,
                    tag.TagNo,
                    tag.TagType);
            });
            return new SuccessResult<IEnumerable<TagDto>>(tagDtos);
        }
    }
}
