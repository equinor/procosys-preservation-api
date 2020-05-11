using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagAttachmentPath
{
    public class GetTagAttachmentPathQueryHandler : IRequestHandler<GetTagAttachmentPathQuery, Result<string>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IBlobPathProvider _blobPathProvider;

        public GetTagAttachmentPathQueryHandler(IReadOnlyContext context, IBlobPathProvider blobPathProvider)
        {
            _context = context;
            _blobPathProvider = blobPathProvider;
        }

        public async Task<Result<string>> Handle(GetTagAttachmentPathQuery request, CancellationToken cancellationToken)
        {
            var attachment = await
                (from a in _context.QuerySet<TagAttachment>()
                    // also join tag to return null if request.TagId not exists
                    join tag in _context.QuerySet<Tag>() on request.TagId equals tag.Id
                    where a.Id == request.AttachmentId
                    select a).SingleOrDefaultAsync(cancellationToken);

            if (attachment == null)
            {
                return new NotFoundResult<string>(
                    $"Tag with ID {request.TagId} or Attachment with ID {request.AttachmentId} not found");
            }

            return new SuccessResult<string>(_blobPathProvider.CreateFullBlobPathForAttachment(attachment));
        }
    }
}
