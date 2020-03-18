using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagFunctionAggregate
{
    public class GetAllTagFunctionsQueryHandler : IRequestHandler<GetAllTagFunctionsQuery, Result<IEnumerable<TagFunctionDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetAllTagFunctionsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<IEnumerable<TagFunctionDto>>> Handle(GetAllTagFunctionsQuery request, CancellationToken cancellationToken)
        {
            var tagFunctions = await (from tagFunction in _context.QuerySet<TagFunction>()
                        .Include(tf => tf.Requirements)
                    select tagFunction)
                .ToListAsync(cancellationToken);

            var tagFunctionDtos =
                tagFunctions.Where(tf => !tf.IsVoided || request.IncludeVoided)
                    .Select(tf => new TagFunctionDto(
                        tf.Id,
                        tf.Code,
                        tf.Description,
                        tf.RegisterCode,
                        tf.Requirements.Where(req => !req.IsVoided || request.IncludeVoided)
                            .Select(s => new RequirementDto(s.Id, s.RequirementDefinitionId))));
            
            return new SuccessResult<IEnumerable<TagFunctionDto>>(tagFunctionDtos);
        }
    }
}
