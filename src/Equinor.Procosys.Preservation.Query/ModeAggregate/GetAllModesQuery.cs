using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetAllModesQuery : IRequest<Result<IEnumerable<ModeDto>>>
    {
        public GetAllModesQuery(bool includeVoided) => IncludeVoided = includeVoided;

        public bool IncludeVoided { get; }
    }
}
