using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagDisciplines
{
    public class GetUniqueTagDisciplinesQueryHandler : IRequestHandler<GetUniqueTagDisciplinesQuery, Result<List<DisciplineDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetUniqueTagDisciplinesQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<DisciplineDto>>> Handle(GetUniqueTagDisciplinesQuery request,
            CancellationToken cancellationToken)
        {
            var disciplines = await
                (from tag in _context.QuerySet<Tag>()
                 join project in _context.QuerySet<Project>()
                     on EF.Property<int>(tag, "ProjectId") equals project.Id
                 where project.Name == request.ProjectName
                       && !string.IsNullOrEmpty(tag.DisciplineCode)
                 select new DisciplineDto(
                     tag.DisciplineCode,
                     tag.DisciplineDescription))
                .Distinct()
                .ToListAsync(cancellationToken);

            return new SuccessResult<List<DisciplineDto>>(disciplines);
        }
    }
}
