﻿using System.Collections.Generic;
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
        Task<bool> HasAnyForSupplierOnlyUsageAsync(List<int> requirementDefinitionIds, CancellationToken token);
        Task<bool> HasAnyFieldsAsync(int requirementDefinitionId, CancellationToken token);
        Task<bool> TagRequirementsExistAsync(int requirementDefinitionId, CancellationToken token);
        Task<bool> TagFunctionRequirementsExistAsync(int requirementDefinitionId, CancellationToken token);
        Task<bool> AllExcludedFieldsIsVoidedAsync(int requirementDefinitionId, List<int> updateFieldIds, CancellationToken token);
        Task<bool> AnyExcludedFieldsIsInUseAsync(int requirementDefinitionId, List<int> updateFieldIds, CancellationToken token);
    }
}
