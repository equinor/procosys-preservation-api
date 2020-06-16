using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetHistory
{
    public class GetHistoryQueryHandler : IRequestHandler<GetHistoryQuery, Result<List<HistoryDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetHistoryQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<HistoryDto>>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)
        {
            var tagHistory = await (from h in _context.QuerySet<History>()
                join tag in _context.QuerySet<Tag>() on h.ObjectGuid equals tag.ObjectGuid
                where tag.Id == request.TagId
                where tag.ObjectGuid == h.ObjectGuid
                select new HistoryDto(h.Id,
                    h.Description,
                    h.CreatedAtUtc,
                    h.CreatedById,
                    h.EventType,
                    null,
                    h.PreservationRecordId)).ToListAsync(cancellationToken);

            return new SuccessResult<List<HistoryDto>>(tagHistory);
        }
    }
}
