using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetAllModesQuery : IRequest<Result<IEnumerable<ModeDto>>>
    {
        public GetAllModesQuery(string plant)
            => Plant = plant ?? throw new ArgumentNullException(nameof(plant));

        public string Plant { get; }
    }
}
