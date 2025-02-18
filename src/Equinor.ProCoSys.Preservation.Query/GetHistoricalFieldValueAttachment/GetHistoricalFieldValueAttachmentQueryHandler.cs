﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Query.UserDelegationProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetHistoricalFieldValueAttachment
{
    public class GetHistoricalFieldValueAttachmentQueryHandler(
        IReadOnlyContext context,
        IAzureBlobService azureBlobService,
        IUserDelegationProvider userDelegationProvider,
        IOptionsSnapshot<BlobStorageOptions> blobStorageOptions)
        : IRequestHandler<GetHistoricalFieldValueAttachmentQuery, Result<Uri>>
    {
        public async Task<Result<Uri>> Handle(GetHistoricalFieldValueAttachmentQuery request, CancellationToken cancellationToken)
        {
            var tag = await
                (from t in context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
                        .ThenInclude(r => r.PreservationPeriods)
                        .ThenInclude(p => p.PreservationRecord)
                        .Include(t => t.Requirements)
                        .ThenInclude(r => r.PreservationPeriods)
                        .ThenInclude(p => p.FieldValues)
                        .ThenInclude(fv => fv.FieldValueAttachment)
                    where t.Id == request.TagId
                    select t).SingleOrDefaultAsync(cancellationToken);

            if (tag == null)
            {
                return new NotFoundResult<Uri>($"{nameof(Tag)} with ID {request.TagId} not found");
            }

            var tagRequirement = tag.Requirements.SingleOrDefault(r => r.Id == request.TagRequirementId);

            if (tagRequirement == null)
            {
                return new NotFoundResult<Uri>($"{nameof(TagRequirement)} with ID {request.TagRequirementId} not found");
            }

            var preservationPeriod = tagRequirement.PreservationPeriods
                .Where(pp => pp.PreservationRecord != null)
                .Where(pp => pp.PreservationRecord.Guid == request.PreservationRecordGuid)
                .Select(pp => pp)
                .SingleOrDefault();


            if (preservationPeriod == null)
            {
                return new NotFoundResult<Uri>($"{nameof(PreservationPeriod)} not found");
            }

            if (preservationPeriod.FieldValues.Count != 1 || preservationPeriod.FieldValues.ToList()[0].FieldValueAttachment == null)
            {
                return new NotFoundResult<Uri>($"{nameof(FieldValue)} with type attachment was not found");
            }

            var fieldValue = preservationPeriod.FieldValues.ToList()[0];

            var attachment = await
                (from a in context.QuerySet<Attachment>()
                 join t in context.QuerySet<Tag>() on request.TagId equals t.Id
                 where a.Id == fieldValue.FieldValueAttachment.Id
                 select a).SingleOrDefaultAsync(cancellationToken);

            if (attachment == null)
            {
                return new NotFoundResult<Uri>($"{nameof(Attachment)} with ID {fieldValue.FieldValueAttachment.Id} not found");
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
