using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class FieldDto
    {
        public FieldDto(Field field, FieldValue currentValue, FieldValue previousValue)
        {
            Id = field.Id;
            Label = field.Label;
            FieldType = field.FieldType;
            Unit = field.Unit;
            ShowPrevious = field.ShowPrevious.HasValue && field.ShowPrevious.Value;
            CurrentValue = CreateFieldValueDto(field.FieldType, currentValue);
            if (ShowPrevious)
            {
                PreviousValue = CreateFieldValueDto(field.FieldType, previousValue);
            }
        }

        public int Id { get; }
        public string Label { get; }
        public FieldType FieldType { get; }
        public string Unit { get; }
        public bool ShowPrevious { get; }
        public object CurrentValue { get; }
        public object PreviousValue { get; }

        private object CreateFieldValueDto(FieldType fieldType, FieldValue fieldValue)
        {
            if (fieldValue == null)
            {
                return null;
            }
            switch (fieldType)
            {
                case FieldType.Number:
                    return new NumberDto(fieldValue as NumberValue);
                case FieldType.CheckBox:
                    return new CheckBoxDto();
                default:
                    return null;
            }
        }
    }
}
