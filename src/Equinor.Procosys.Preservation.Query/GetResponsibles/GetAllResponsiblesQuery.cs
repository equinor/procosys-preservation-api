using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetResponsibles
{
    public class GetAllResponsiblesQuery : IRequest<Result<IEnumerable<ResponsibleDto>>>
    {
    }
}
