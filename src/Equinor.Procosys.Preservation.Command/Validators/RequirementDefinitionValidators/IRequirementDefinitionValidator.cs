using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators
{
    public interface IRequirementDefinitionValidator
    {
        Task<bool> ExistsAsync(int requirementDefinitionId, CancellationToken token);
        Task<bool> IsVoidedAsync(int requirementDefinitionId, CancellationToken token);
        Task<bool> UsageIsBothForSupplierAndOtherAsync(List<int> requirementDefinitionIds, CancellationToken token);
        Task<bool> UsageIsForOtherOnlyAsync(List<int> requirementDefinitionIds, CancellationToken token);
        Task<bool> NoUsageIsForSupplierOnlyAsync(List<int> requirementDefinitionIds, CancellationToken token);
    }
}
