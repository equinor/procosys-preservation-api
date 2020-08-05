using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public interface IRowVersionValidator
    {
        Task<bool> IsValid(string rowVersion, CancellationToken cancellationToken);
    }
}
