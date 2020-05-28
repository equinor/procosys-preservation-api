using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetModes
{
    public class GetAllModesQuery : IRequest<Result<IEnumerable<ModeDto>>>
    {
    }
}
