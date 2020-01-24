using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class GetAllTagsInProjectQueryHandler : IRequestHandler<GetAllTagsInProjectQuery, Result<IEnumerable<TagDto>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITimeService _timeService;

        public GetAllTagsInProjectQueryHandler(IProjectRepository projectRepository, ITimeService timeService)
        {
            _projectRepository = projectRepository;
            _timeService = timeService;
        }

        public async Task<Result<IEnumerable<TagDto>>> Handle(GetAllTagsInProjectQuery request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetAllTagsInProjectAsync(request.ProjectName);
            return new SuccessResult<IEnumerable<TagDto>>(tags.Select(tag =>
            {
                var now = _timeService.GetCurrentTimeUtc();
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

                return new TagDto(tag.Id,
                    tag.AreaCode,
                    tag.Calloff,
                    tag.CommPkgNo,
                    tag.DisciplineCode,
                    firstUpcommingRequirementDto,
                    tag.IsAreaTag,
                    tag.IsVoided,
                    tag.McPkgNo,
                    tag.NeedUserInput,
                    tag.PurchaseOrderNo,
                    requirementsDtos,
                    tag.Status,
                    tag.StepId,
                    tag.TagFunctionCode,
                    tag.Description,
                    tag.TagNo);
            }));
        }
    }
}
