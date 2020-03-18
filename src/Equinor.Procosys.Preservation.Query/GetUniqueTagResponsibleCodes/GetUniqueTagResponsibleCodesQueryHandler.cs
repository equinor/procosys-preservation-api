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

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibleCodes
{
    public class GetUniqueTagResponsibleCodesQueryHandler : IRequestHandler<GetUniqueTagResponsibleCodesQuery, Result<List<ResponsibleDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetUniqueTagResponsibleCodesQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<ResponsibleDto>>> Handle(GetUniqueTagResponsibleCodesQuery request, CancellationToken cancellationToken)
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
                     responsible.Title))
                .Distinct()
                .ToListAsync(cancellationToken);
            
            return new SuccessResult<List<ResponsibleDto>>(responsibleCodes);
        }
    }
}
