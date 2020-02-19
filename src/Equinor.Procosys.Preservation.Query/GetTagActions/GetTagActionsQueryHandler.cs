using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
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
            // Get tag with all actions and all comments
            var tag = await
                (from t in _context.QuerySet<Tag>()
                        .Include(t => t.Actions)
                        .ThenInclude(a => a.ActionComments)
                    where t.Id == request.Id
                    select t).FirstOrDefaultAsync(cancellationToken);
            
            if (tag == null)
            {
                return new NotFoundResult<List<ActionDto>>($"Entity with ID {request.Id} not found");
            }

            // Get
            var personIds = tag.Actions.SelectMany(a => a.ActionComments).Select(ac => ac.CommentedById).Distinct().ToList();
            var persons = await
                (from p in _context.QuerySet<Person>()
                    where personIds.Contains(p.Id)
                    select p).ToListAsync(cancellationToken);

            var actions = tag
                .Actions
                .Select(action =>
                {
                    var comments = action
                        .ActionComments
                        .Select(ac =>
                        {
                            var person = persons.SingleOrDefault(p => p.Id == ac.CommentedById);
                            return new ActionCommentDto(
                                ac.Id,
                                ac.CommentedAtUtc,
                                person != null
                                    ? new PersonDto(
                                        person.Id,
                                        person.FirstName,
                                        person.LastName)
                                    : null);
                        })
                        .ToList();

                    return new ActionDto(
                        action.Id,
                        action.Description,
                        action.DueTimeUtc,
                        comments);
                }).ToList();
            
            return new SuccessResult<List<ActionDto>>(actions);
        }
    }
}
