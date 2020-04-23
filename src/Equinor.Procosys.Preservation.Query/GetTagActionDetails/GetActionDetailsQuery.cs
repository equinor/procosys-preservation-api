using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagActionDetails
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
