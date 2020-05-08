using System.IO;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.GetTagAttachmentStream
{
    public class GetTagAttachmentStreamQuery : IRequest<AttachmentStreamDto>, ITagQueryRequest
    {
        public GetTagAttachmentStreamQuery(int tagId, int attachmentId, Stream openStream)
        {
            TagId = tagId;
            AttachmentId = attachmentId;
            OpenStream = openStream;
        }

        public int TagId { get; }
        public int AttachmentId { get; }
        public Stream OpenStream { get; }
    }
}
