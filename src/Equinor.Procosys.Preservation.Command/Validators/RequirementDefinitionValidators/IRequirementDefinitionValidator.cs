using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators
{
    public interface IRequirementDefinitionValidator
    {
        Task<bool> ExistsAsync(int requirementDefinitionId, CancellationToken token);
        Task<bool> IsVoidedAsync(int requirementDefinitionId, CancellationToken token);
        Task<bool> UsageCoversBothForSupplierAndOtherAsync(List<int> requirementDefinitionIds, CancellationToken token);
        Task<bool> UsageCoversForOtherThanSuppliersAsync(List<int> requirementDefinitionIds, CancellationToken token);
        Task<bool> UsageCoversForSupplierOnlyAsync(List<int> requirementDefinitionIds, CancellationToken token);
        Task<bool> BeUniqueRequirements(IEnumerable<int> updatedTagReqIds, IEnumerable<int> newReqDefIds, CancellationToken token);
        Task<bool> RequirementUsageIsForAllJourneysAsync(IEnumerable<int> updatedTagReqIds, IEnumerable<int> newReqDefIds, CancellationToken token);
        Task<bool> RequirementUsageIsForJourneysWithoutSupplierAsync(IEnumerable<int> updatedTagReqIds, IEnumerable<int> newReqDefIds, CancellationToken token);
        Task<bool> RequirementUsageIsNotForSupplierStepOnlyAsync(IEnumerable<int> updatedTagReqIds, IEnumerable<int> newReqDefIds, CancellationToken token);
    }
}
