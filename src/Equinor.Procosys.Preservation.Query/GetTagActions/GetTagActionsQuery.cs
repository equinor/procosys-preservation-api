using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagActions
{
    public class GetTagActionsQuery : IRequest<Result<List<ActionDto>>>
    {
        public GetTagActionsQuery(string plant, int id)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            Id = id;
        }

        public string Plant { get; }
        public int Id { get; }
    }
}
