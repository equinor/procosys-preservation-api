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

            return new SuccessResult<ModeDto>(new ModeDto(mode.Id, mode.Title, mode.RowVersion.ConvertToString()));
        }
    }
}
