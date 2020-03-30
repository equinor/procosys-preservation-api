using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetAreas
{
    public class GetAreasQuery : IRequest<Result<List<AreaDto>>>
    {
        public GetAreasQuery(string plant)
            => Plant = plant ?? throw new ArgumentNullException(nameof(plant));

        public string Plant { get; }
    }
}
