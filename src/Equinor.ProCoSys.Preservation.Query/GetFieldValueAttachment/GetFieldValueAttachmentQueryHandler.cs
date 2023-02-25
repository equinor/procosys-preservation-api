using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Auth.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetFieldValueAttachment
{
    public class GetFieldValueAttachmentQueryHandler : IRequestHandler<GetFieldValueAttachmentQuery, Result<Uri>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsSnapshot<BlobStorageOptions> _blobStorageOptions;

        public GetFieldValueAttachmentQueryHandler(IReadOnlyContext context, IBlobStorage blobStorage, IOptionsSnapshot<BlobStorageOptions> blobStorageOptions)
        {
            _context = context;
            _blobStorage = blobStorage;
            _blobStorageOptions = blobStorageOptions;
        }

        public async Task<Result<Uri>> Handle(GetFieldValueAttachmentQuery request, CancellationToken cancellationToken)
        {
            var tag = await
                (from t in _context.QuerySet<Tag>()
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

            var requirement = tag.Requirements.Single(r => r.Id == request.RequirementId);

            var requirementDefinition = await
                (from rd in _context.QuerySet<RequirementDefinition>().Include(rd => rd.Fields)
                    where rd.Id == requirement.RequirementDefinitionId
                    select rd
                ).SingleOrDefaultAsync(cancellationToken);

            var attachment = requirement.GetAlreadyRecordedAttachment(request.FieldId, requirementDefinition);

            if (attachment == null)
            {
                return new NotFoundResult<Uri>($"{nameof(Tag)} with ID {request.TagId} or attachment not found for field {request.FieldId}");
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
