using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.AttachmentValidators
{
    public interface IAttachmentValidator
    {
        Task<bool> AttachmentExistsAsync(int attachmentId, CancellationToken token);
    }
}
