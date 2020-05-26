using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class GetAllJourneysQueryHandler : IRequestHandler<GetAllJourneysQuery, Result<IEnumerable<JourneyDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetAllJourneysQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<IEnumerable<JourneyDto>>> Handle(GetAllJourneysQuery request,
            CancellationToken cancellationToken)
        {
            var journeys = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                select j).ToListAsync(cancellationToken);

            var modeIds = journeys.SelectMany(j => j.Steps).Select(x => x.ModeId).Distinct();
            var responsibleIds = journeys.SelectMany(j => j.Steps).Select(x => x.ResponsibleId).Distinct();

            var modes = await (from m in _context.QuerySet<Mode>()
                where modeIds.Contains(m.Id)
                select m).ToListAsync(cancellationToken);

            var responsibles = await (from r in _context.QuerySet<Responsible>()
                where responsibleIds.Contains(r.Id)
                select r).ToListAsync(cancellationToken);

            var journeyDtos =
                journeys.Where(j => !j.IsVoided || request.IncludeVoided)
                    .Select(j => new JourneyDto(
                        j.Id,
                        j.Title,
                        j.IsVoided,
                        j.OrderedSteps().Where(s => !s.IsVoided || request.IncludeVoided)
                            .Select(s =>
                            {
                                var modeDto = modes
                                    .Where(m => m.Id == s.ModeId)
                                    .Select(m => new ModeDto(m.Id, m.Title, m.RowVersion.ConvertToString()))
                                    .Single();
                                var responsibleDto = responsibles
                                    .Where(r => r.Id == s.ResponsibleId)
                                    .Select(r => new ResponsibleDto(r.Id, r.Code, r.Title,
                                        r.RowVersion.ConvertToString()))
                                    .Single();
                                return new StepDto(
                                    s.Id,
                                    s.Title,
                                    s.IsVoided,
                                    s.RowVersion.ConvertToString(),
                                    modeDto, responsibleDto);
                            }),
                        j.RowVersion.ConvertToString()));

            return new SuccessResult<IEnumerable<JourneyDto>>(journeyDtos);
        }
    }
}
