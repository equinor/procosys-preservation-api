using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetActionDetails
{
    public class GetActionDetailsQuery : IRequest<Result<ActionDetailsDto>>, ITagQueryRequest
    {
        public GetActionDetailsQuery(int tagId, int actionId)
        {
            TagId = tagId;
            ActionId = actionId;
        }

        public int TagId { get; }
        public int ActionId { get; }
    }
}
