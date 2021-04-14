using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.ModeAggregate
{
    public class GetModeByIdQueryHandler : IRequestHandler<GetModeByIdQuery, Result<ModeDto>>
    {
        private readonly IReadOnlyContext _context;

        public GetModeByIdQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<ModeDto>> Handle(GetModeByIdQuery request, CancellationToken cancellationToken)
        {
            var mode = await (from m in _context.QuerySet<Mode>()
                where m.Id == request.Id
                select m).SingleOrDefaultAsync(cancellationToken);

            if (mode == null)
            {
                return new NotFoundResult<ModeDto>(Strings.EntityNotFound(nameof(Mode), request.Id));
            }

            var inUse = await (from s in _context.QuerySet<Step>()
                where s.ModeId == mode.Id
                select s).AnyAsync(cancellationToken);

            return new SuccessResult<ModeDto>(new ModeDto(mode.Id, mode.Title, mode.IsVoided, mode.ForSupplier, inUse, mode.RowVersion.ConvertToString()));
        }
    }
}
