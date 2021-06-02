using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetActionsCrossPlant
{
    public class GetActionsCrossPlantQuery : IRequest<Result<List<ActionDto>>>, ICrossPlantQueryRequest
    {
    }
}
