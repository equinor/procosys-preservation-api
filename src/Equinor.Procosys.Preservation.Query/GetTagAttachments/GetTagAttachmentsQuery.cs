using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetTagAttachments
{
    public class GetTagAttachmentsQuery : IRequest<Result<List<TagAttachmentDto>>>, ITagQueryRequest
    {
        public GetTagAttachmentsQuery(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
