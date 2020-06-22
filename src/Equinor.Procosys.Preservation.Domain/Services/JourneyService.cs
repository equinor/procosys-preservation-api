using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Domain.Services
{
    public class JourneyService : IJourneyService
    {
        private readonly IReadOnlyContext _context;

        public JourneyService(IReadOnlyContext context) => _context = context;

        public async Task<bool> IsJourneyInUseAsync(long journeyId, CancellationToken cancellationToken)
        {
            var stepIds = await (from step in _context.QuerySet<Step>()
                where EF.Property<int>(step, "JourneyId") == journeyId
                select step.Id).ToListAsync(cancellationToken: cancellationToken);

            var inUse = await (from tag in _context.QuerySet<Tag>()
                where stepIds.Contains(tag.StepId)
                select tag).AnyAsync(cancellationToken);
            return inUse;
        }
    }
}
