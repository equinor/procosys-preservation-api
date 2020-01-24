using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class GetAllResponsiblesQuery : IRequest<Result<IEnumerable<ResponsibleDto>>>
    {
    }
}
