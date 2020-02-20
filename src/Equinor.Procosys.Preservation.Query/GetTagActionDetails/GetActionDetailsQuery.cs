using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagActionDetails
{
    public class GetActionDetailsQuery : IRequest<Result<ActionDetailsDto>>
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
