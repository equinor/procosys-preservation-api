using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.JourneyValidators
{
    public class journeyValidator : IJourneyValidator
    {
        private readonly IReadOnlyContext _context;

        public journeyValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int journeyId, CancellationToken token) =>
            await (from j in _context.QuerySet<Journey>()
                where j.Id == journeyId
                select j).AnyAsync(token);

        public async Task<bool> ExistsWithSameTitleAsync(string journeyTitle, CancellationToken token) =>
            await (from j in _context.QuerySet<Journey>()
                where j.Title == journeyTitle
                select j).AnyAsync(token);

        public async Task<bool> ExistsWithSameTitleInAnotherJourneyAsync(int journeyId, string journeyTitle, CancellationToken token) =>
            await (from j in _context.QuerySet<Journey>()
                where j.Id != journeyId && j.Title == journeyTitle
                select j).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(int journeyId, CancellationToken token)
        {
            var journey = await (from j in _context.QuerySet<Journey>()
                where j.Id == journeyId
                select j).SingleOrDefaultAsync(token);
            return journey != null && journey.IsVoided;
        }
    }
}
