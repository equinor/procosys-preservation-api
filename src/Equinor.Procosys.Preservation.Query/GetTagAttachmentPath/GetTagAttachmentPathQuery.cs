using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagAttachmentPath
{
    public class GetTagAttachmentPathQuery : IRequest<Result<string>>, ITagQueryRequest
    {
        public GetTagAttachmentPathQuery(int tagId, int attachmentId)
        {
            TagId = tagId;
            AttachmentId = attachmentId;
        }

        public int TagId { get; }
        public int AttachmentId { get; }
    }
}
