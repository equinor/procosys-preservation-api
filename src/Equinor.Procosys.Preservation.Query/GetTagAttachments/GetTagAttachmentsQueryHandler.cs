using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagAttachments
{
    public class GetTagAttachmentsQueryHandler : IRequestHandler<GetTagAttachmentsQuery, Result<List<TagAttachmentDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetTagAttachmentsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<TagAttachmentDto>>> Handle(GetTagAttachmentsQuery request, CancellationToken cancellationToken)
        {
            // Get tag with all actions
            var tag = await
                (from t in _context.QuerySet<Tag>()
                        .Include(t => t.Attachments)
                    where t.Id == request.TagId
                    select t).SingleOrDefaultAsync(cancellationToken);
            
            if (tag == null)
            {
                return new NotFoundResult<List<TagAttachmentDto>>($"Entity with ID {request.TagId} not found");
            }

            var actions = tag
                .Attachments
                .Select(attachment => new TagAttachmentDto(
                    attachment.Id,
                    attachment.Title,
                    attachment.FileName)).ToList();
            
            return new SuccessResult<List<TagAttachmentDto>>(actions);
        }
    }
}
