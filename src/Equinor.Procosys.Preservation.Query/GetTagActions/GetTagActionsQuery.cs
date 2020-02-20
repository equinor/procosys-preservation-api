using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagActions
{
    public class GetTagActionsQuery : IRequest<Result<List<ActionDto>>>
    {
        public GetTagActionsQuery(int id) => Id = id;

        public int Id { get; }
    }
}
