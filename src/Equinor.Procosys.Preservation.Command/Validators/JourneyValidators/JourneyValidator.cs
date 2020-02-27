using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.JourneyValidators
{
    public class JourneyValidator : IJourneyValidator
    {
        private readonly IReadOnlyContext _context;

        public JourneyValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int journeyId, CancellationToken token)
        {
            var count = await (from j in _context.QuerySet<Journey>()
                where j.Id == journeyId
                select j).CountAsync(token);
            return count > 0;
        }

        public async Task<bool> ExistsAsync(string title, CancellationToken token)
        {
            var count = await (from j in _context.QuerySet<Journey>()
                where j.Title == title
                select j).CountAsync(token);
            return count > 0;
        }

        public async Task<bool> IsVoidedAsync(int journeyId, CancellationToken token)
        {
            var journey = await (from j in _context.QuerySet<Journey>()
                where j.Id == journeyId
                select j).FirstOrDefaultAsync(token);
            return journey != null && journey.IsVoided;
        }
    }
}
