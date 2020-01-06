using System.Collections.Generic;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class GetAllRequirementTypesQuery : IRequest<IEnumerable<RequirementTypeDto>>
    {
        public GetAllRequirementTypesQuery(bool includeVoided) => IncludeVoided = includeVoided;

        public bool IncludeVoided { get; }
    }
}
