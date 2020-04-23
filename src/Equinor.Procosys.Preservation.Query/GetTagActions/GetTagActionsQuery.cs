using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagActions
{
    public class GetTagActionsQuery : IRequest<Result<List<ActionDto>>>, ITagQueryRequest
    {
        public GetTagActionsQuery(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
