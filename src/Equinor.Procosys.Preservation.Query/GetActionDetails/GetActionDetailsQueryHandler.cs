using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Query.GetActionDetails
{
    public class GetActionDetailsQueryHandler : IRequestHandler<GetActionDetailsQuery, Result<ActionDetailsDto>>
    {
        private readonly IReadOnlyContext _context;

        public GetActionDetailsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<ActionDetailsDto>> Handle(GetActionDetailsQuery request, CancellationToken cancellationToken)
        {
            var dto = await
                (from a in _context.QuerySet<Action>().Include(a => a.Attachments)
                     // also join tag to return null if request.TagId not exists
                     join tag in _context.QuerySet<Tag>() on EF.Property<int>(a, "TagId") equals tag.Id
                 join createdUser in _context.QuerySet<Person>()
                     on EF.Property<int>(a, "CreatedById") equals createdUser.Id
                 from closedUser in _context.QuerySet<Person>()
                     .Where(p => p.Id == EF.Property<int>(a, "ClosedById")).DefaultIfEmpty() // left join
                 where tag.Id == request.TagId && a.Id == request.ActionId
                 select new Dto
                 {
                     Action = a,
                     CreatedBy = createdUser,
                     ClosedBy = closedUser,
                     AttachmentCount = a.Attachments.ToList().Count
                 }).SingleOrDefaultAsync(cancellationToken);

            if (dto == null)
            {
                return new NotFoundResult<ActionDetailsDto>($"Tag with ID {request.TagId} or Action with ID {request.ActionId} not found");
            }

            var createdBy = new PersonDto(dto.CreatedBy.Id, dto.CreatedBy.FirstName, dto.CreatedBy.LastName);
            PersonDto closedBy = null;
            if (dto.ClosedBy != null)
            {
                closedBy = new PersonDto(dto.ClosedBy.Id, dto.ClosedBy.FirstName, dto.ClosedBy.LastName);
            }

            var action = new ActionDetailsDto(
                dto.Action.Id,
                createdBy,
                dto.Action.CreatedAtUtc,
                dto.Action.Title,
                dto.Action.Description,
                dto.Action.DueTimeUtc,
                dto.Action.IsClosed,
                closedBy,
                dto.Action.ClosedAtUtc,
                dto.AttachmentCount,
                dto.Action.RowVersion.ConvertToString());
            
            return new SuccessResult<ActionDetailsDto>(action);
        }

        private class Dto
        {
            public Action Action { get; set; }
            public Person CreatedBy { get; set; }
            public Person ClosedBy { get; set; }
            public int AttachmentCount { get; set; }
        }
    }
}
