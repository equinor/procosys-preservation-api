using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagFunctions
{
    public class GetUniqueTagFunctionsQueryHandler : IRequestHandler<GetUniqueTagFunctionsQuery, Result<List<TagFunctionCodeDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetUniqueTagFunctionsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<TagFunctionCodeDto>>> Handle(GetUniqueTagFunctionsQuery request,
            CancellationToken cancellationToken)
        {
            var tagFunctions = await
                (from tagFunction in _context.QuerySet<TagFunction>()
                    join project in _context.QuerySet<Project>() on tagFunction.Plant equals project.Plant
                    where project.Name == request.ProjectName
                          && !string.IsNullOrEmpty(tagFunction.Code) && _context.QuerySet<TagFunctionRequirement>().Any(tfr => EF.Property<int>(tfr, "TagFunctionId") == tagFunction.Id)
                    select new TagFunctionCodeDto(
                        tagFunction.Code,
                        tagFunction.Description)
                    )
                .Distinct()
                .ToListAsync(cancellationToken);

            return new SuccessResult<List<TagFunctionCodeDto>>(tagFunctions);
        }
    }
}
