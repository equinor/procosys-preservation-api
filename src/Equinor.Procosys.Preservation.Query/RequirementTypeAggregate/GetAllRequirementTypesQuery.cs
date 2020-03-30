using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class GetAllRequirementTypesQuery : IRequest<Result<IEnumerable<RequirementTypeDto>>>
    {
        public GetAllRequirementTypesQuery(string plant, bool includeVoided)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            IncludeVoided = includeVoided;
        }

        public string Plant { get; }
        public bool IncludeVoided { get; }
    }
}
