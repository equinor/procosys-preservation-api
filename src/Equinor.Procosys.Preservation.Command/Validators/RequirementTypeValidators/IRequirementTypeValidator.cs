using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators
{
    public interface IRequirementTypeValidator
    {
        Task<bool> ExistsAsync(int requirementTypeId, CancellationToken token);
        Task<bool> RequirementDefinitionExistsAsync(int requirementTypeId, CancellationToken token);
        Task<bool> IsVoidedAsync(int requirementTypeId, CancellationToken token);
        Task<bool> IsNotUniqueCodeAsync(string code, CancellationToken token);
        Task<bool> IsNotUniqueTitleAsync(string title, CancellationToken token);
        Task<bool> IsNotUniqueCodeAsync(int requirementTypeId, string code, CancellationToken token);
        Task<bool> IsNotUniqueTitleAsync(int requirementTypeId, string title, CancellationToken token);
    }
}
