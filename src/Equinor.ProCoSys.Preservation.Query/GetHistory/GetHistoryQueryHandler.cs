using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetHistory
{
    public class GetHistoryQueryHandler : IRequestHandler<GetHistoryQuery, Result<List<HistoryDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetHistoryQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<HistoryDto>>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)
        {
            // Get tag with all actions
            var tag = await
                (from t in _context.QuerySet<Tag>()
                 where t.Id == request.TagId
                 select t).SingleOrDefaultAsync(cancellationToken);

            if (tag == null)
            {
                return new NotFoundResult<List<HistoryDto>>($"Entity with ID {request.TagId} not found");
            }

            var tagHistory = await (from h in _context.QuerySet<History>()
                                    join t in _context.QuerySet<Tag>() on h.SourceGuid equals t.Guid
                                    join createdBy in _context.QuerySet<Person>() on h.CreatedById equals createdBy.Id
                                    from preservationRecord in _context.QuerySet<PreservationRecord>()
                                        .Where(pr => pr.Guid == EF.Property<Guid>(h, "PreservationRecordGuid")).DefaultIfEmpty() //left join
                                    from preservationPeriod in _context.QuerySet<PreservationPeriod>()
                                        .Where(pr => pr.PreservationRecord.Id == EF.Property<int>(preservationRecord, "Id")).DefaultIfEmpty() // left join
                                    where t.Id == request.TagId
                                    select new HistoryDto(
                                        h.Id,
                                        h.Description,
                                        h.CreatedAtUtc,
                                        new PersonDto(createdBy.Id, createdBy.FirstName, createdBy.LastName),
                                        h.EventType,
                                        h.DueInWeeks,
                                        preservationPeriod.TagRequirementId,
                                        h.PreservationRecordGuid)
                ).ToListAsync(cancellationToken);

            var tagHistoryOrdered = tagHistory.OrderByDescending(h => h.CreatedAtUtc).ToList();

            return new SuccessResult<List<HistoryDto>>(tagHistoryOrdered);
        }
    }
}
