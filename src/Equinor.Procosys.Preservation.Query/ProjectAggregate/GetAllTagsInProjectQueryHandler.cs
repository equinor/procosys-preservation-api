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

            var stepIds = tags.Select(t => t.StepId).Distinct();
            var steps = await _journeyRepository.GetStepsByStepIdsAsync(stepIds);

            var modeIds = steps.Select(s => s.ModeId).Distinct();
            var modes = await _modeRepository.GetByIdsAsync(modeIds);

            var respIds = steps.Select(s => s.ResponsibleId).Distinct();
            var responsibles = await _responsibleRepository.GetByIdsAsync(respIds);

            var now = _timeService.GetCurrentTimeUtc();

            return new SuccessResult<IEnumerable<TagDto>>(tags.Select(tag =>
            {
                var requirementsDtos = tag.Requirements.Select(
                    r => new RequirementDto(
                        r.Id,
                        r.RequirementDefinitionId,
                        now,
                        r.NextDueTimeUtc,
                        r.ReadyToBePreserved))
                    .ToList();

                var firstUpcomingRequirement = tag.FirstUpcomingRequirement;

                RequirementDto firstUpcomingRequirementDto = null;
                if (firstUpcomingRequirement != null)
                {
                    firstUpcomingRequirementDto = requirementsDtos.Single(r => r.Id == firstUpcomingRequirement.Id);
                }

                var step = steps.Single(s => s.Id == tag.StepId);
                var mode = modes.Single(m => m.Id == step.ModeId);
                var resp = responsibles.Single(r => r.Id == step.ResponsibleId);

                return new TagDto(tag.Id,
                    tag.AreaCode,
                    tag.Calloff,
                    tag.CommPkgNo,
                    tag.DisciplineCode,
                    firstUpcomingRequirementDto,
                    tag.IsAreaTag,
                    tag.IsVoided,
                    tag.McPkgNo,
                    mode.Title,
                    tag.ReadyToBePreserved,
                    tag.PurchaseOrderNo,
                    tag.Remark,
                    requirementsDtos,
                    resp.Name,
                    tag.Status,
                    tag.TagFunctionCode,
                    tag.Description,
                    tag.TagNo);
            }));
        }
    }
}
