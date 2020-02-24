using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagActions
{
    public class GetTagActionsQueryHandler : IRequestHandler<GetTagActionsQuery, Result<List<ActionDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetTagActionsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<ActionDto>>> Handle(GetTagActionsQuery request, CancellationToken cancellationToken)
        {
            // Get tag with all actions
            var tag = await
                (from t in _context.QuerySet<Tag>()
                        .Include(t => t.Actions)
                    where t.Id == request.Id
                    select t).FirstOrDefaultAsync(cancellationToken);
            
            if (tag == null)
            {
                return new NotFoundResult<List<ActionDto>>($"Entity with ID {request.Id} not found");
            }

            var actions = tag
                .Actions
                .Select(action => new ActionDto(
                    action.Id,
                    action.Title,
                    action.DueTimeUtc,
                    action.IsClosed)).ToList();
            
            return new SuccessResult<List<ActionDto>>(actions);
        }
    }
}
