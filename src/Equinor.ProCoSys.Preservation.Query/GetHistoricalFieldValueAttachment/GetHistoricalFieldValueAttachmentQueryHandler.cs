using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetHistoricalFieldValueAttachment
{
    public class GetHistoricalFieldValueAttachmentQueryHandler : IRequestHandler<GetHistoricalFieldValueAttachmentQuery, Result<Uri>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsMonitor<BlobStorageOptions> _blobStorageOptions;

        public GetHistoricalFieldValueAttachmentQueryHandler(
            IReadOnlyContext context,
            IBlobStorage blobStorage,
            IOptionsMonitor<BlobStorageOptions> blobStorageOptions)
        {
            _context = context;
            _blobStorage = blobStorage;
            _blobStorageOptions = blobStorageOptions;
        }

        public async Task<Result<Uri>> Handle(GetHistoricalFieldValueAttachmentQuery request, CancellationToken cancellationToken)
        {
            var tag = await
                (from t in _context.QuerySet<Tag>()
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

            var tagRequirement = tag.Requirements.Single(r => r.Id == request.TagRequirementId);

            if (tagRequirement == null)
            {
                return new NotFoundResult<Uri>($"{nameof(TagRequirement)} with ID {request.TagRequirementId} not found");
            }

            var preservationPeriod = tagRequirement.PreservationPeriods
                .Where(pp => pp.PreservationRecord != null)
                .Where(pp => pp.PreservationRecord.ObjectGuid == request.PreservationRecordGuid)
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
                (from a in _context.QuerySet<Attachment>()
                 join t in _context.QuerySet<Tag>() on request.TagId equals t.Id
                 where a.Id == fieldValue.FieldValueAttachment.Id
                 select a).SingleOrDefaultAsync(cancellationToken);

            var now = TimeService.UtcNow;
            var fullBlobPath = attachment.GetFullBlobPath(_blobStorageOptions.CurrentValue.BlobContainer);
            
            var uri = _blobStorage.GetDownloadSasUri(
                fullBlobPath,
                new DateTimeOffset(now.AddMinutes(_blobStorageOptions.CurrentValue.BlobClockSkewMinutes * -1)),
                new DateTimeOffset(now.AddMinutes(_blobStorageOptions.CurrentValue.BlobClockSkewMinutes)));
            return new SuccessResult<Uri>(uri);
        }
    }
}
