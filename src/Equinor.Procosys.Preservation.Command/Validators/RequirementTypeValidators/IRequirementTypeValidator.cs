using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators
{
    public interface IRequirementTypeValidator
    {
        Task<bool> ExistsAsync(int requirementTypeId, CancellationToken token);
        Task<bool> AnyRequirementDefinitionExistsAsync(int requirementTypeId, CancellationToken token);
        Task<bool> RequirementDefinitionExistsAsync(int requirementTypeId, int requirementDefinitionId, CancellationToken token);
        Task<bool> IsVoidedAsync(int requirementTypeId, CancellationToken token);
        Task<bool> ExistsWithSameCodeAsync(string code, CancellationToken token);
        Task<bool> ExistsWithSameTitleAsync(string title, CancellationToken token);
        Task<bool> ExistsWithSameCodeInAnotherTypeAsync(int requirementTypeId, string code, CancellationToken token);
        Task<bool> ExistsWithSameTitleInAnotherTypeAsync(int requirementTypeId, string title, CancellationToken token);
        Task<bool> AnyRequirementDefinitionExistsWithSameTitleAsync(
            int requirementTypeId, 
            string reqDefTitle,
            IList<FieldType> fieldTypes,
            CancellationToken token);
        Task<bool> OtherRequirementDefinitionExistsWithSameTitleAsync(
            int requirementTypeId,
            int requirementDefinitionId,
            string reqDefTitle,
            IList<FieldType> fieldTypes,
            CancellationToken token);
    }
}
