using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetAreaCodes
{
    public class GetAreaCodesQuery : IRequest<Result<List<AreaCodeDto>>>
    {
    }
}
