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

        public bool VerifyFieldType(int fieldId, FieldType fieldType)
        {
            var field = _requirementTypeRepository.GetFieldByIdAsync(fieldId).Result;
            return field != null && field.FieldType == fieldType;
        }

        public bool IsValidValue(int fieldId, string value)
        {
            var field = _requirementTypeRepository.GetFieldByIdAsync(fieldId).Result;
            switch (field.FieldType)
            {
                case FieldType.Number:
                    return IsAValidNumberValue(value);
                case FieldType.CheckBox:
                    return IsAValidCheckBoxValue(value);
                default:
                    return false;
            }
        }

        private bool IsAValidCheckBoxValue(string value)
            => bool.TryParse(value, out _);

        private bool IsAValidNumberValue(string value)
        {
            // NA and N/A is legal special cases for a number
            if (value.ToUpper() == "NA" || value.ToUpper() == "N/A")
            {
                return true;
            }

            return double.TryParse(value, out _);
        }
    }
}
