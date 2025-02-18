using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Query.UserDelegationProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.GetActionAttachment
{
    public class GetActionAttachmentQueryHandler(
        IReadOnlyContext context,
        IAzureBlobService azureBlobService,
        IUserDelegationProvider userDelegationProvider,
        IOptionsSnapshot<BlobStorageOptions> blobStorageOptions)
        : IRequestHandler<GetActionAttachmentQuery, Result<Uri>>
    {
        public async Task<Result<Uri>> Handle(GetActionAttachmentQuery request, CancellationToken cancellationToken)
        {
            var attachment = await
                (from a in context.QuerySet<ActionAttachment>()
                    // also join action to return null if request.ActionId not exists
                 join action in context.QuerySet<Action>() on request.ActionId equals action.Id
                 join tag in context.QuerySet<Tag>() on request.TagId equals tag.Id
                 where a.Id == request.AttachmentId
                 select a).SingleOrDefaultAsync(cancellationToken);

            if (attachment == null)
            {
                return new NotFoundResult<Uri>($"Action with ID {request.ActionId} or Attachment with ID {request.AttachmentId} not found");
            }

            var now = TimeService.UtcNow;
            var fullBlobPath = attachment.GetFullBlobPath();
            var uri = azureBlobService.GetDownloadSasUri(
                blobStorageOptions.Value.BlobContainer,
                fullBlobPath,
                new DateTimeOffset(now.AddMinutes(blobStorageOptions.Value.BlobClockSkewMinutes * -1)),
                new DateTimeOffset(now.AddMinutes(blobStorageOptions.Value.BlobClockSkewMinutes)),
                userDelegationProvider.GetUserDelegationKey());
            return new SuccessResult<Uri>(uri);
        }
    }
}
