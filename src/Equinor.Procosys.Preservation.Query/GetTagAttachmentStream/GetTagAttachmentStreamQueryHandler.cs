using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Query.GetTagAttachmentStream
{
    public class GetTagAttachmentStreamQueryHandler : IRequestHandler<GetTagAttachmentStreamQuery, AttachmentStreamDto>
    {
        private readonly IReadOnlyContext _context;
        private readonly IBlobStorage _blobStorage;
        private readonly IBlobPathProvider _blobPathProvider;

        public GetTagAttachmentStreamQueryHandler(IReadOnlyContext context, IBlobStorage blobStorage, IBlobPathProvider blobPathProvider)
        {
            _context = context;
            _blobStorage = blobStorage;
            _blobPathProvider = blobPathProvider;
        }

        public async Task<AttachmentStreamDto> Handle(GetTagAttachmentStreamQuery request, CancellationToken cancellationToken)
        {
            var attachment = await
                (from a in _context.QuerySet<TagAttachment>()
                    // also join tag to return null if request.TagId not exists
                    join tag in _context.QuerySet<Tag>() on request.TagId equals tag.Id
                    where a.Id == request.AttachmentId
                    select a).SingleOrDefaultAsync(cancellationToken);

            if (attachment == null)
            {
                return null;
            }

            var dto = new AttachmentStreamDto(attachment.Id, attachment.FileName, request.OpenStream);
            var path = _blobPathProvider.CreateFullBlobPathForAttachment(attachment);
            if (await _blobStorage.DownloadAsync(path, dto.Content, cancellationToken))

            {
                return dto;
            }

            throw new Exception($"Not able to download {attachment.FileName}");
        }
    }
}
