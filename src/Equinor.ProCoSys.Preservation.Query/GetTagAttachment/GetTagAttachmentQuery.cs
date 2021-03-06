﻿using System;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetTagAttachment
{
    public class GetTagAttachmentQuery : IRequest<Result<Uri>>, ITagQueryRequest
    {
        public GetTagAttachmentQuery(int tagId, int attachmentId)
        {
            TagId = tagId;
            AttachmentId = attachmentId;
        }

        public int TagId { get; }
        public int AttachmentId { get; }
    }
}
