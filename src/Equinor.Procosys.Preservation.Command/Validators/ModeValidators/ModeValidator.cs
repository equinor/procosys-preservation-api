using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators
{
    public class ModeValidator : IModeValidator
    {
        private readonly IReadOnlyContext _context;

        public ModeValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int modeId, CancellationToken token) =>
            await (from m in _context.QuerySet<Mode>()
                where m.Id == modeId
                select m).AnyAsync(token);

        public async Task<bool> ExistsWithSameTitleAsync(string title, CancellationToken token) =>
            await (from m in _context.QuerySet<Mode>()
                where m.Title == title
                select m).AnyAsync(token);

        public async Task<bool> ExistsAnotherModeWithSameTitleAsync(int modeId, string title, CancellationToken token) =>
            await (from m in _context.QuerySet<Mode>()
                where m.Title == title && m.Id != modeId
                select m).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(int modeId, CancellationToken token)
        {
            var mode = await (from m in _context.QuerySet<Mode>()
                where m.Id == modeId
                select m).SingleOrDefaultAsync(token);
            return mode != null && mode.IsVoided;
        }

        public async Task<bool> IsUsedInStepAsync(int modeId, CancellationToken token) =>
            await (from s in _context.QuerySet<Domain.AggregateModels.JourneyAggregate.Step>()
                where s.ModeId == modeId
                select s).AnyAsync(token);

        public async Task<bool> IsForSupplierAsync(int modeId, CancellationToken token)
        {
            var mode = await (from m in _context.QuerySet<Mode>()
                where m.Id == modeId
                select m).SingleOrDefaultAsync(token);
            return mode != null && mode.ForSupplier;
        }

        public async Task<bool> ExistsAnotherModeForSupplierAsync(int modeId, CancellationToken token) => 
            await (from m in _context.QuerySet<Mode>()
                          where m.Id != modeId &&
                                m.ForSupplier
                          select m).AnyAsync(token);

        public async Task<bool> ExistsModeForSupplierAsync(CancellationToken token) =>
            await (from m in _context.QuerySet<Mode>()
                where m.ForSupplier
                select m).AnyAsync(token);
    }
}
