using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetHistory
{
    public class GetHistoryQuery : IRequest<Result<List<HistoryDto>>>, ITagQueryRequest
    {
        public GetHistoryQuery(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
