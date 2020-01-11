using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public class RequirementValidator : IRequirementValidator
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public RequirementValidator(IRequirementTypeRepository requirementTypeRepository)
            => _requirementTypeRepository = requirementTypeRepository;

        public bool Exists(int requirementDefinitionId)
            => _requirementTypeRepository.GetRequirementDefinitionByIdAsync(requirementDefinitionId).Result != null;
    }
}
