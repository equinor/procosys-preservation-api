using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetActionAttachments
{
    public class GetActionAttachmentsQuery : IRequest<Result<List<ActionAttachmentDto>>>, ITagQueryRequest
    {
        public GetActionAttachmentsQuery(int tagId, int actionId)
        {
            TagId = tagId;
            ActionId = actionId;
        }

        public int TagId { get; }
        public int ActionId { get; }
    }
}
