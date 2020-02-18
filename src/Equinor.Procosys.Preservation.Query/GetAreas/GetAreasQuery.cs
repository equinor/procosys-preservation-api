using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetAreas
{
    public class GetAreasQuery : IRequest<Result<List<AreaDto>>>
    {
    }
}
