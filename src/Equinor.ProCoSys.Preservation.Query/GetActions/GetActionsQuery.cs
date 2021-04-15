using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetActions
{
    public class GetActionsQuery : IRequest<Result<List<ActionDto>>>, ITagQueryRequest
    {
        public GetActionsQuery(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
