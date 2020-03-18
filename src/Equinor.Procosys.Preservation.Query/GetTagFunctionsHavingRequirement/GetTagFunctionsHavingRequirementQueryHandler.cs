using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagFunctionsHavingRequirement
{
    public class GetTagFunctionsHavingRequirementQueryHandler : IRequestHandler<GetTagFunctionsHavingRequirementQuery, Result<IEnumerable<TagFunctionDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetTagFunctionsHavingRequirementQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<IEnumerable<TagFunctionDto>>> Handle(GetTagFunctionsHavingRequirementQuery request, CancellationToken cancellationToken)
        {
            var tagFunctionDtos = await (from tagFunction in _context.QuerySet<TagFunction>().Include(tf => tf.Requirements)
                    where tagFunction.Requirements.Any()
                    select new TagFunctionDto(tagFunction.Id, tagFunction.Code, tagFunction.RegisterCode))
                .ToListAsync(cancellationToken);

            return new SuccessResult<IEnumerable<TagFunctionDto>>(tagFunctionDtos);
        }
    }
}
