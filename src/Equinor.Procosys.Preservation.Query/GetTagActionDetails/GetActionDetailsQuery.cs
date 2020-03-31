using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagActionDetails
{
    public class GetActionDetailsQuery : IRequest<Result<ActionDetailsDto>>
    {
        public GetActionDetailsQuery(string plant, int tagId, int actionId)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            TagId = tagId;
            ActionId = actionId;
        }

        public string Plant { get; }
        public int TagId { get; }
        public int ActionId { get; }
    }
}
