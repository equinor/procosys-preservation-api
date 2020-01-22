using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class AllTagsInProjectQueryHandler : IRequestHandler<AllTagsInProjectQuery, Result<IEnumerable<TagDto>>>
    {
        private readonly IProjectRepository _projectRepository;

        public AllTagsInProjectQueryHandler(IProjectRepository projectRepository) => _projectRepository = projectRepository;

        public async Task<Result<IEnumerable<TagDto>>> Handle(AllTagsInProjectQuery request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetAllTagsInProjectAsync(request.ProjectName);
            return new SuccessResult<IEnumerable<TagDto>>(tags.Select(tag =>
                new TagDto(tag.Id,
                tag.AreaCode,
                tag.Calloff,
                tag.CommPkgNo,
                tag.DisciplineCode,
                tag.IsAreaTag,
                tag.IsVoided,
                tag.McPkgNo,
                tag.NeedUserInput,
                tag.ProjectName,
                tag.PurchaseOrderNo,
                tag.Requirements.Select(r =>
                        new RequirementDto(r.NextDueTimeUtc)),
                tag.Status,
                tag.StepId,
                tag.TagFunctionCode,
                tag.TagNo)));
        }
    }
}
