using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetActionsCrossPlant
{
    public class GetActionsCrossPlantQuery : IRequest<Result<List<ActionDto>>>
    {
        public GetActionsCrossPlantQuery(int max = 0) => Max = max;

        public int Max { get; }
    }
}
