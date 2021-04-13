using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Command.Validators.ResponsibleValidators
{
    public interface IResponsibleValidator
    {
        Task<bool> ExistsAndIsVoidedAsync(string responsibleCode, CancellationToken cancellationToken);
    }
}
