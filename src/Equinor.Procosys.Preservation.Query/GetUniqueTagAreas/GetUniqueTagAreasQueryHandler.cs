using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagAreas
{
    public class GetUniqueTagAreasQueryHandler : IRequestHandler<GetUniqueTagAreasQuery, Result<List<AreaCodeDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetUniqueTagAreasQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<AreaCodeDto>>> Handle(GetUniqueTagAreasQuery request,
            CancellationToken cancellationToken)
        {
            var areas = await
                (from tag in _context.QuerySet<Tag>()
                    join project in _context.QuerySet<Project>()
                        on EF.Property<int>(tag, "ProjectId") equals project.Id
                    where project.Name == request.ProjectName
                          && !string.IsNullOrEmpty(tag.AreaCode)
                    select new AreaCodeDto(
                        tag.AreaCode,
                        tag.AreaDescription))
                .Distinct()
                .ToListAsync(cancellationToken);

            return new SuccessResult<List<AreaCodeDto>>(areas);
        }
    }
}
