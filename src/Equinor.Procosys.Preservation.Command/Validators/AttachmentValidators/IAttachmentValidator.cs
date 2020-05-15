using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.AttachmentValidators
{
    public interface IAttachmentValidator
    {
        Task<bool> ExistsAsync(int attachmentId, CancellationToken token);
    }
}
