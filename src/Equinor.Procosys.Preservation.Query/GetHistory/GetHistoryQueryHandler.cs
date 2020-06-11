using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetHistory
{
    public class GetHistoryQueryHandler : IRequestHandler<GetHistoryQuery, Result<List<HistoryDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetHistoryQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<HistoryDto>>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)        {
            var tagHistory = await
                (from h in _context.QuerySet<History>()
                    where h.ObjectId == request.TagId
                    select new HistoryDto(
                        h.Id,
                        h.Description,
                        h.CreatedAtUtc,
                        h.CreatedById,
                        null, 
                        h.PreservationRecordId))
                .ToListAsync(cancellationToken);

            return new SuccessResult<List<HistoryDto>>(tagHistory);
        }
    }
}
