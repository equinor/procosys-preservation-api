using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class GetAllJourneysQuery : IRequest<Result<IEnumerable<JourneyDto>>>
    {
        public GetAllJourneysQuery(string plant, bool includeVoided)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            IncludeVoided = includeVoided;
        }

        public string Plant { get; }
        public bool IncludeVoided { get; }
    }
}
