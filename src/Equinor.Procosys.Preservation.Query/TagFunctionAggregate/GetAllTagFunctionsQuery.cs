using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagFunctionAggregate
{
    public class GetAllTagFunctionsQuery : IRequest<Result<IEnumerable<TagFunctionDto>>>
    {
        public GetAllTagFunctionsQuery(bool includeVoided) => IncludeVoided = includeVoided;

        public bool IncludeVoided { get; }
    }
}
