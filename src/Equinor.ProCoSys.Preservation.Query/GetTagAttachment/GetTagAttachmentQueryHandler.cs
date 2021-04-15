using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using HeboTech.TimeService;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetTagAttachment
{
    public class GetTagAttachmentQueryHandler : IRequestHandler<GetTagAttachmentQuery, Result<Uri>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

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

            var now = TimeService.Now;
            var fullBlobPath = attachment.GetFullBlobPath(_attachmentOptions.CurrentValue.BlobContainer);
            
            var uri = _blobStorage.GetDownloadSasUri(
                fullBlobPath,
                new DateTimeOffset(now.AddMinutes(_attachmentOptions.CurrentValue.BlobClockSkewMinutes * -1)),
                new DateTimeOffset(now.AddMinutes(_attachmentOptions.CurrentValue.BlobClockSkewMinutes)));
            return new SuccessResult<Uri>(uri);
        }
    }
}
