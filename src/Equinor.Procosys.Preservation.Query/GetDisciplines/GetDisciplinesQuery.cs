using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetDisciplines
{
    public class GetDisciplinesQuery : IRequest<Result<List<DisciplineDto>>>
    {
        public GetDisciplinesQuery(string plant)
            => Plant = plant ?? throw new ArgumentNullException(nameof(plant));

        public string Plant { get; }
    }
}
