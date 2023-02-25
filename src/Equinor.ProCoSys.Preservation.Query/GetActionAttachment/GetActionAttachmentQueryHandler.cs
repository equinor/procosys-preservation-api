using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Auth.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.GetActionAttachment
{
    public class GetActionAttachmentQueryHandler : IRequestHandler<GetActionAttachmentQuery, Result<Uri>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsSnapshot<BlobStorageOptions> _blobStorageOptions;

        public GetActionAttachmentQueryHandler(IReadOnlyContext context, IBlobStorage blobStorage, IOptionsSnapshot<BlobStorageOptions> blobStorageOptions)
        {
            _context = context;
            _blobStorage = blobStorage;
            _blobStorageOptions = blobStorageOptions;
        }

        public async Task<Result<Uri>> Handle(GetActionAttachmentQuery request, CancellationToken cancellationToken)
        {
            var attachment = await
                (from a in _context.QuerySet<ActionAttachment>()
                    // also join action to return null if request.ActionId not exists
                 join action in _context.QuerySet<Action>() on request.ActionId equals action.Id
                 join tag in _context.QuerySet<Tag>() on request.TagId equals tag.Id
                 where a.Id == request.AttachmentId
                 select a).SingleOrDefaultAsync(cancellationToken);

            if (attachment == null)
            {
                return new NotFoundResult<Uri>($"Action with ID {request.ActionId} or Attachment with ID {request.AttachmentId} not found");
            }

            var now = TimeService.UtcNow;
            var fullBlobPath = attachment.GetFullBlobPath(_blobStorageOptions.Value.BlobContainer);

            var uri = _blobStorage.GetDownloadSasUri(
                fullBlobPath,
                new DateTimeOffset(now.AddMinutes(_blobStorageOptions.Value.BlobClockSkewMinutes * -1)),
                new DateTimeOffset(now.AddMinutes(_blobStorageOptions.Value.BlobClockSkewMinutes)));
            return new SuccessResult<Uri>(uri);
        }
    }
}
