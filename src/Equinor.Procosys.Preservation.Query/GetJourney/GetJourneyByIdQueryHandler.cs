using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Query.GetMode;
using Equinor.Procosys.Preservation.Query.GetResponsibles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetJourney
{
    public class GetJourneyByIdQueryHandler : IRequestHandler<GetJourneyByIdQuery, Result<JourneyDto>>
    {
        private readonly IReadOnlyContext _context;

        public GetJourneyByIdQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<JourneyDto>> Handle(GetJourneyByIdQuery request, CancellationToken cancellationToken)
        {
            var journey = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                where j.Id == request.Id
                select j).SingleOrDefaultAsync(cancellationToken);
            if (journey == null)
            {
                return new NotFoundResult<JourneyDto>(Strings.EntityNotFound(nameof(Journey), request.Id));
            }

            var modeIds = journey.Steps.Select(x => x.ModeId);
            var responsibleIds = journey.Steps.Select(x => x.ResponsibleId);

            var modes = await (from m in _context.QuerySet<Mode>()
                    where modeIds.Contains(m.Id)
                    select new ModeDto(m.Id, m.Title, m.RowVersion.ConvertToString()))
                .ToListAsync(cancellationToken);
            var responsibles = await (from r in _context.QuerySet<Responsible>()
                    where responsibleIds.Contains(r.Id)
                    select new ResponsibleDto(r.Id, r.Code, r.Title, r.RowVersion.ConvertToString()))
                .ToListAsync(cancellationToken);

            var journeyDto = new JourneyDto(
                journey.Id,
                journey.Title,
                journey.IsVoided,
                journey.Steps.Select(step =>
                    new StepDto(
                        step.Id,
                        step.Title,
                        step.IsVoided,
                        modes.FirstOrDefault(x => x.Id == step.ModeId),
                        responsibles.FirstOrDefault(x => x.Id == step.ResponsibleId),
                        step.RowVersion.ConvertToString()
                    )
                ),
                journey.RowVersion.ConvertToString());
            return new SuccessResult<JourneyDto>(journeyDto);
        }
    }
}
