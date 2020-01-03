using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class AllResponsiblesQuery : IRequest<Result<List<ResponsibleDto>>>
    {
    }
}
