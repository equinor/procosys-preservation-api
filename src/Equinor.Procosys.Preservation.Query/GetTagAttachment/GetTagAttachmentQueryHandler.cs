﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagAttachment
{
    public class GetTagAttachmentQueryHandler : IRequestHandler<GetTagAttachmentQuery, Result<Uri>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

        // todo unit test
        public GetTagAttachmentQueryHandler(IReadOnlyContext context, IBlobStorage blobStorage, IOptionsMonitor<AttachmentOptions> attachmentOptions)
        {
            _context = context;
            _blobStorage = blobStorage;
            _attachmentOptions = attachmentOptions;
        }

        public async Task<Result<Uri>> Handle(GetTagAttachmentQuery request, CancellationToken cancellationToken)
        {
            var attachment = await
                (from a in _context.QuerySet<TagAttachment>()
                    // also join tag to return null if request.TagId not exists
                    join tag in _context.QuerySet<Tag>() on request.TagId equals tag.Id
                    where a.Id == request.AttachmentId
                    select a).SingleOrDefaultAsync(cancellationToken);

            if (attachment == null)
            {
                return new NotFoundResult<Uri>($"Tag with ID {request.TagId} or Attachment with ID {request.AttachmentId} not found");
            }

            var now = TimeService.UtcNow;
            var fullBlobPathForAttachment = attachment.GetFullBlobPath(_attachmentOptions.CurrentValue.BlobContainer);
            
            var uri = _blobStorage.GetDownloadSasUri(
                fullBlobPathForAttachment,
                new DateTimeOffset(now.AddMinutes(_attachmentOptions.CurrentValue.BlobClockSkewMinutes * -1)),
                new DateTimeOffset(now.AddMinutes(_attachmentOptions.CurrentValue.BlobClockSkewMinutes)));
            return new SuccessResult<Uri>(uri);
        }
    }
}
