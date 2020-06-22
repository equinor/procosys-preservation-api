using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibles
{
    public class GetUniqueTagResponsiblesQueryHandler : IRequestHandler<GetUniqueTagResponsiblesQuery, Result<List<ResponsibleDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetUniqueTagResponsiblesQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<ResponsibleDto>>> Handle(GetUniqueTagResponsiblesQuery request, CancellationToken cancellationToken)
        {
            var responsibleCodes = await
                (from responsible in _context.QuerySet<Responsible>()
                        join step in _context.QuerySet<Step>()
                            on responsible.Id equals step.ResponsibleId
                        join tag in _context.QuerySet<Tag>()
                            on step.Id equals tag.StepId
                        join project in _context.QuerySet<Project>()
                            on EF.Property<int>(tag, "ProjectId") equals project.Id
                 where project.Name == request.ProjectName
                 select new ResponsibleDto(
                     responsible.Id,
                     responsible.Code,
                     responsible.Description))
                .Distinct()
                .ToListAsync(cancellationToken);
            
            return new SuccessResult<List<ResponsibleDto>>(responsibleCodes);
        }
    }
}
