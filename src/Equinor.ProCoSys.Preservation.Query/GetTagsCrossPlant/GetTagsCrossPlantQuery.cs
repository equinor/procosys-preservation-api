using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant
{
    public class GetTagsCrossPlantQuery : IRequest<Result<List<TagDto>>>, ICrossPlantQueryRequest
    {
    }
}
