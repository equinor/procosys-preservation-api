using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.ActionValidators
{
    public class ActionValidator : IActionValidator
    {
        private readonly IReadOnlyContext _context;

        public ActionValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> IsClosedAsync(int actionId, CancellationToken token)
        {
            var action = await (from a in _context.QuerySet<Action>()
                          where a.Id == actionId
                          select a).SingleOrDefaultAsync(token);
            return action != null && action.IsClosed;
        }

        public async Task<bool> AttachmentWithFilenameExistsAsync(int actionId, string fileName, CancellationToken token)
        {
            var action = await (from a in _context.QuerySet<Action>().Include(a => a.Attachments)
                where a.Id == actionId
                select a).SingleOrDefaultAsync(token);

            return action?.GetAttachmentByFileName(fileName) != null;
        }
    }
}
