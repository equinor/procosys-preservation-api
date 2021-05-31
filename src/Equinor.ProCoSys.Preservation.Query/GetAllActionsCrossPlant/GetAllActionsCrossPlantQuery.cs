using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetAllActionsCrossPlant
{
    public class GetAllActionsCrossPlantQuery : IRequest<Result<List<ActionDto>>>, ICrossPlantQueryRequest
    {
    }
}
