using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class AllTagsQueryHandler : IRequestHandler<AllTagsQuery, Result<IEnumerable<TagDto>>>
    {
        private readonly ITagRepository _tagRepository;

        public AllTagsQueryHandler(ITagRepository tagRepository) => _tagRepository = tagRepository;

        public async Task<Result<IEnumerable<TagDto>>> Handle(AllTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _tagRepository.GetAllAsync();
            return new SuccessResult<IEnumerable<TagDto>>(tags.Select(tag =>
                new TagDto(tag.Id,
                tag.AreaCode,
                tag.Calloff,
                tag.CommPkgNo,
                tag.DisciplineCode,
                tag.IsAreaTag,
                tag.IsVoided,
                tag.McPkgNo,
                tag.ProjectName,
                tag.PurchaseOrderNo,
                tag.Requirements.Select(r =>
                        new RequirementDto(r.Id,
                            r.RequirementDefinitionId,
                            r.IsVoided,
                            r.IntervalWeeks)),
                tag.Status,
                tag.StepId,
                tag.TagFunctionCode,
                tag.TagNo)));
        }
    }
}
