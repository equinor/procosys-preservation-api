using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagAreas
{
    public class GetUniqueTagAreasQueryHandler : IRequestHandler<GetUniqueTagAreasQuery, Result<List<AreaDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetUniqueTagAreasQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<AreaDto>>> Handle(GetUniqueTagAreasQuery request,
            CancellationToken cancellationToken)
        {
            var areas = await
                (from tag in _context.QuerySet<Tag>()
                    join project in _context.QuerySet<Project>()
                        on EF.Property<int>(tag, "ProjectId") equals project.Id
                    where project.Name == request.ProjectName
                          && !string.IsNullOrEmpty(tag.AreaCode)
                    select new AreaDto(
                        tag.AreaCode,
                        tag.AreaDescription))
                .Distinct()
                .ToListAsync(cancellationToken);

            return new SuccessResult<List<AreaDto>>(areas);
        }
    }
}
