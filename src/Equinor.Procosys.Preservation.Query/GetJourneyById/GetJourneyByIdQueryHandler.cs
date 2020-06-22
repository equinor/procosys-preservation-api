using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Services;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetJourneyById
{
    public class GetJourneyByIdQueryHandler : IRequestHandler<GetJourneyByIdQuery, Result<JourneyDetailsDto>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IJourneyService _journeyService;

        public GetJourneyByIdQueryHandler(IReadOnlyContext context, IJourneyService journeyService)
        {
            _context = context;
            _journeyService = journeyService;
        }

        public async Task<Result<JourneyDetailsDto>> Handle(GetJourneyByIdQuery request, CancellationToken cancellationToken)
        {
            var journey = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                where j.Id == request.Id
                select j).SingleOrDefaultAsync(cancellationToken);
            if (journey == null)
            {
                return new NotFoundResult<JourneyDetailsDto>(Strings.EntityNotFound(nameof(Journey), request.Id));
            }

            var journeyInUse = await _journeyService.IsJourneyInUseAsync(request.Id, cancellationToken);

            var modeIds = journey.Steps.Select(x => x.ModeId);
            var responsibleIds = journey.Steps.Select(x => x.ResponsibleId);

            var modes = await (from m in _context.QuerySet<Mode>()
                    where modeIds.Contains(m.Id)
                    select new ModeDto(m.Id, m.Title, m.ForSupplier, m.RowVersion.ConvertToString()))
                .ToListAsync(cancellationToken);
            var responsibles = await (from r in _context.QuerySet<Responsible>()
                    where responsibleIds.Contains(r.Id)
                    select new ResponsibleDto(r.Id, r.Code, r.Description, r.RowVersion.ConvertToString()))
                .ToListAsync(cancellationToken);

            var journeyDto = new JourneyDetailsDto(
                journey.Id,
                journey.Title,
                journeyInUse,
                journey.IsVoided,
                journey.OrderedSteps().Select(step =>
                    new StepDetailsDto(
                        step.Id,
                        step.Title,
                        step.IsVoided,
                        modes.FirstOrDefault(x => x.Id == step.ModeId),
                        responsibles.FirstOrDefault(x => x.Id == step.ResponsibleId),
                        step.RowVersion.ConvertToString()
                    )
                ),
                journey.RowVersion.ConvertToString());
            return new SuccessResult<JourneyDetailsDto>(journeyDto);
        }
    }
}
