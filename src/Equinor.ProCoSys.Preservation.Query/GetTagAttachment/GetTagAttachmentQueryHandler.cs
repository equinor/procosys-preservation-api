using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Common.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetTagAttachment
{
    public class GetTagAttachmentQueryHandler : IRequestHandler<GetTagAttachmentQuery, Result<Uri>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IOptionsSnapshot<BlobStorageOptions> _blobStorageOptions;

        public GetTagAttachmentQueryHandler(IReadOnlyContext context, IAzureBlobService azureBlobService, IOptionsSnapshot<BlobStorageOptions> blobStorageOptions)
        {
            _context = context;
            _azureBlobService = azureBlobService;
            _blobStorageOptions = blobStorageOptions;
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
            var fullBlobPath = attachment.GetFullBlobPath();
            var uri = _azureBlobService.GetDownloadSasUri(
                _blobStorageOptions.Value.BlobContainer,
                fullBlobPath,
                new DateTimeOffset(now.AddMinutes(_blobStorageOptions.Value.BlobClockSkewMinutes * -1)),
                new DateTimeOffset(now.AddMinutes(_blobStorageOptions.Value.BlobClockSkewMinutes)));
            return new SuccessResult<Uri>(uri);
        }
    }
}
