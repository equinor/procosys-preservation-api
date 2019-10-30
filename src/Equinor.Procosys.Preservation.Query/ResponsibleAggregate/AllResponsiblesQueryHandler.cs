using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class AllResponsiblesQueryHandler : IRequestHandler<AllResponsiblesQuery, List<ResponsibleDto>>
    {
        public Task<List<ResponsibleDto>> Handle(AllResponsiblesQuery request, CancellationToken cancellationToken)
        {
            // TODO: This is hard-coded test data.
            return Task.FromResult(new List<ResponsibleDto>
            {
                new ResponsibleDto { Name = "ABC" },
                new ResponsibleDto { Name = "DEF" }
            });
        }
    }
}
