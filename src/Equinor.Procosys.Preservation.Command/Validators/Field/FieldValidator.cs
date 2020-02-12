using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
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

        public bool IsValidValue(int fieldId, string value)
        {
            var field = _requirementTypeRepository.GetFieldByIdAsync(fieldId).Result;
            switch (field.FieldType)
            {
                case FieldType.Number:
                    return NumberValue.IsValidValue(value, out _);
                case FieldType.CheckBox:
                    return CheckBoxChecked.IsValidValue(value, out _);
                default:
                    return false;
            }
        }
    }
}
