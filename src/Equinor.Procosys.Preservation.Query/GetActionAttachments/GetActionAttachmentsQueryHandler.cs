using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetActionAttachments
{
    public class GetActionAttachmentsQueryHandler : IRequestHandler<GetActionAttachmentsQuery, Result<List<ActionAttachmentDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetActionAttachmentsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<ActionAttachmentDto>>> Handle(GetActionAttachmentsQuery request, CancellationToken cancellationToken)
        {
            // Get action with all attachments
            var action = await
                (from a in _context.QuerySet<Action>()
                        .Include(t => t.Attachments)
                    // also join tag to return null if request.TagId not exists
                    join tag in _context.QuerySet<Tag>() on EF.Property<int>(a, "TagId") equals tag.Id
                    where tag.Id == request.TagId && a.Id == request.ActionId
                    select a).SingleOrDefaultAsync(cancellationToken);
            
            if (action == null)
            {
                return new NotFoundResult<List<ActionAttachmentDto>>($"Action with ID {request.ActionId} not found in Tag {request.TagId}");
            }

            var attachments = action
                .Attachments
                .Select(attachment => new ActionAttachmentDto(
                    attachment.Id,
                    attachment.FileName)).ToList();
            
            return new SuccessResult<List<ActionAttachmentDto>>(attachments);
        }
    }
}
