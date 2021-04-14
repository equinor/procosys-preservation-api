using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.ResponsibleAggregate
{
    public class GetAllResponsiblesQuery : IRequest<Result<IEnumerable<ResponsibleDto>>>
    {
    }
}
