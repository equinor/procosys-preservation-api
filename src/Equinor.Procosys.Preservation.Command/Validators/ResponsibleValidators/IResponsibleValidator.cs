using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.ResponsibleValidators
{
    public interface IResponsibleValidator
    {
        Task<bool> ExistsAsync(int responsibleId, CancellationToken token);

        Task<bool> IsVoidedAsync(int responsibleId, CancellationToken token);
    }
}
