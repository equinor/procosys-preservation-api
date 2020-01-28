using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Field
{
    public class FieldValidator : IFieldValidator
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public FieldValidator(IRequirementTypeRepository requirementTypeRepository)
            => _requirementTypeRepository = requirementTypeRepository;

        public bool Exists(int fieldId)
            => _requirementTypeRepository.GetFieldByIdAsync(fieldId).Result != null;

        public bool IsVoided(int fieldId)
        {
            var field = _requirementTypeRepository.GetFieldByIdAsync(fieldId).Result;
            return field != null && field.IsVoided;
        }
    }
}
