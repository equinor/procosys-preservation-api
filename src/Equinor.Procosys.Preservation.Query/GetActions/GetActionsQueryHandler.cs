using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetActions
{
    public class GetActionsQueryHandler : IRequestHandler<GetActionsQuery, Result<List<ActionDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetActionsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<ActionDto>>> Handle(GetActionsQuery request, CancellationToken cancellationToken)
        {
            // Get tag with all actions
            var tag = await
                (from t in _context.QuerySet<Tag>()
                        .Include(t => t.Actions)
                        .ThenInclude(a => a.Attachments)
                    where t.Id == request.TagId
                    select t).SingleOrDefaultAsync(cancellationToken);
            
            if (tag == null)
            {
                return new NotFoundResult<List<ActionDto>>($"Entity with ID {request.TagId} not found");
            }

            var actions = tag
                .Actions
                .OrderBy(t => t.IsClosed.Equals(true))
                .ThenByDescending(t => t.DueTimeUtc.HasValue)
                .ThenBy(t => t.DueTimeUtc)
                .ThenBy(t => t.ModifiedAtUtc)
                .ThenBy(t => t.CreatedAtUtc)
                .Select(action => new ActionDto(
                    action.Id,
                    action.Title,
                    action.DueTimeUtc,
                    action.IsClosed,
                    action.Attachments.ToList().Count,
                    action.RowVersion.ConvertToString())).ToList();
            return new SuccessResult<List<ActionDto>>(actions);
        }
    }
}
