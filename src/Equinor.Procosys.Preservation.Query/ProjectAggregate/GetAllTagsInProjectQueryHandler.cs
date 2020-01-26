using System;
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
            var tags = await _projectRepository.GetAllTagsInProjectAsync(request.ProjectName);

            if (!tags.Any())
            {
                return new SuccessResult<IEnumerable<TagDto>>(new List<TagDto>());
            }

            var stepIds = tags.Select(t => t.StepId).Distinct();
            var steps = await _journeyRepository.GetStepsByStepIdsAsync(stepIds);

            var modeIds = steps.Select(s => s.ModeId).Distinct();
            var modes = await _modeRepository.GetByIdsAsync(modeIds);

            var respIds = steps.Select(s => s.ResponsibleId).Distinct();
            var resposibles = await _responsibleRepository.GetByIdsAsync(respIds);

            var now = _timeService.GetCurrentTimeUtc();

            return new SuccessResult<IEnumerable<TagDto>>(tags.Select(tag =>
            {
                var requirementsDtos = tag.Requirements.Select(
                    r => new RequirementDto(
                        r.Id,
                        r.RequirementDefinitionId,
                        r.NextDueTimeUtc,
                        r.GetTimeUntilNextDueTime(now)))
                    .ToList();

                var firstUpcommingRequirement = tag.FirstUpcommingRequirement;

                RequirementDto firstUpcommingRequirementDto = null;
                if (firstUpcommingRequirement != null)
                {
                    firstUpcommingRequirementDto = requirementsDtos.Single(r => r.Id == firstUpcommingRequirement.Id);
                }

                var step = steps.Single(s => s.Id == tag.StepId);
                var mode = modes.Single(m => m.Id == step.ModeId);
                var resp = resposibles.Single(r => r.Id == step.ResponsibleId);

                /* Above or this solution Henning? :
                var step = steps.FirstOrDefault(s => s.Id == tag.StepId);
                if (step == null)
                {
                    throw new Exception($"Data inconsistency: Step {tag.StepId} in Tag {tag.Id} do not exists");
                }
                var mode = modes.FirstOrDefault(m => m.Id == step.ModeId);
                if (mode == null)
                {
                    throw new Exception($"Data inconsistency: Mode {step.ModeId} in Step {step.Id} do not exists");
                }
                var resp = resposibles.FirstOrDefault(r => r.Id == step.ResponsibleId);
                if (resp == null)
                {
                    throw new Exception($"Data inconsistency: Responsible {step.ResponsibleId} in Step {step.Id} do not exists");
                }
                */

                return new TagDto(tag.Id,
                    tag.AreaCode,
                    tag.Calloff,
                    tag.CommPkgNo,
                    tag.DisciplineCode,
                    firstUpcommingRequirementDto,
                    tag.IsAreaTag,
                    tag.IsVoided,
                    tag.McPkgNo,
                    mode.Title,
                    tag.NeedUserInput,
                    tag.PurchaseOrderNo,
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
