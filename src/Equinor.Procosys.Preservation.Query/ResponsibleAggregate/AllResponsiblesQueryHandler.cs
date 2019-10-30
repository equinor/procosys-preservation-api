using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class AllResponsiblesQueryHandler : IRequestHandler<AllResponsiblesQuery, List<ResponsibleDto>>
    {
        public Task<List<ResponsibleDto>> Handle(AllResponsiblesQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
