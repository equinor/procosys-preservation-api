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
        Task<bool> BeUniqueRequirements(IEnumerable<int> updatedRequirements, IEnumerable<int> newRequirements, CancellationToken token);
        Task<bool> RequirementUsageIsForAllJourneysAsync(IEnumerable<int> updatedRequirements, IEnumerable<int> newRequirements, CancellationToken token);
        Task<bool> RequirementUsageIsForJourneysWithoutSupplierAsync(IEnumerable<int> updatedRequirements, IEnumerable<int> newRequirements, CancellationToken token);
        Task<bool> RequirementUsageIsNotForSupplierStepOnlyAsync(IEnumerable<int> updatedRequirements, IEnumerable<int> newRequirements, CancellationToken token);
        Task<List<int>> GetAllUpdatedReqDefIds(IEnumerable<int> updatedRequirements, CancellationToken token);
    }
}
