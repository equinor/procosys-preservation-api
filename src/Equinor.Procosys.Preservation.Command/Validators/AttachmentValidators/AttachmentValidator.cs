using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.AttachmentValidators
{
    public class AttachmentValidator : IAttachmentValidator
    {
        private readonly IReadOnlyContext _context;

        public AttachmentValidator(IReadOnlyContext context) => _context = context;

        // unit tests
        public async Task<bool> AttachmentExistsAsync(int attachmentId, CancellationToken token) =>
            await (from a in _context.QuerySet<Attachment>()
                where a.Id == attachmentId
                select a).AnyAsync(token);
    }
}
