using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetAllModesQueryHandler : IRequestHandler<GetAllModesQuery, Result<IEnumerable<ModeDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetAllModesQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<IEnumerable<ModeDto>>> Handle(GetAllModesQuery request, CancellationToken cancellationToken)
        {
            var modes = await (from m in _context.QuerySet<Mode>()
                select m).ToListAsync(cancellationToken);
            return new SuccessResult<IEnumerable<ModeDto>>(modes.Select(mode => new ModeDto(mode.Id, mode.Title, mode.RowVersion.ToULong())));
        }
    }
}
