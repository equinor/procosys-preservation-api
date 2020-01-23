using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.RequirementDefinition
{
    public class RequirementDefinitionValidator : IRequirementDefinitionValidator
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public RequirementDefinitionValidator(IRequirementTypeRepository requirementTypeRepository)
            => _requirementTypeRepository = requirementTypeRepository;

        public bool Exists(int requirementDefinitionId)
            => _requirementTypeRepository.GetRequirementDefinitionByIdAsync(requirementDefinitionId).Result != null;

        public bool IsVoided(int requirementDefinitionId)
        {
            var r = _requirementTypeRepository.GetRequirementDefinitionByIdAsync(requirementDefinitionId).Result;
            return r != null && r.IsVoided;
        }
    }
}
