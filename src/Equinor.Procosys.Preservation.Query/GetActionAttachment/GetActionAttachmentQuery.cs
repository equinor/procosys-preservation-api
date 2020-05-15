using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetActionAttachment
{
    public class GetActionAttachmentQuery : IRequest<Result<Uri>>, ITagQueryRequest
    {
        public GetActionAttachmentQuery(int tagId, int actionId, int attachmentId)
        {
            TagId = tagId;
            ActionId = actionId;
            AttachmentId = attachmentId;
        }

        public int TagId { get; }
        public int ActionId { get; }
        public int AttachmentId { get; }
    }
}
