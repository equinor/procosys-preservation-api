using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class GetAllResponsiblesQuery : IRequest<Result<IEnumerable<ResponsibleDto>>>
    {
        public GetAllResponsiblesQuery(string plant)
            => Plant = plant ?? throw new ArgumentNullException(nameof(plant));

        public string Plant { get; }
    }
}
