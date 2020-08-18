using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Query.GetActionAttachment
{
    public class GetActionAttachmentQueryHandler : IRequestHandler<GetActionAttachmentQuery, Result<Uri>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

        public GetActionAttachmentQueryHandler(IReadOnlyContext context, IBlobStorage blobStorage, IOptionsMonitor<AttachmentOptions> attachmentOptions)
        {
            _context = context;
            _blobStorage = blobStorage;
            _attachmentOptions = attachmentOptions;
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
            var fullBlobPath = attachment.GetFullBlobPath(_attachmentOptions.CurrentValue.BlobContainer);

            var uri = _blobStorage.GetDownloadSasUri(
                fullBlobPath,
                new DateTimeOffset(now.AddMinutes(_attachmentOptions.CurrentValue.BlobClockSkewMinutes * -1)),
                new DateTimeOffset(now.AddMinutes(_attachmentOptions.CurrentValue.BlobClockSkewMinutes)));
            return new SuccessResult<Uri>(uri);
        }
    }
}
