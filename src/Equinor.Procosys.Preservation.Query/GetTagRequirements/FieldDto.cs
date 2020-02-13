using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class FieldDto
    {
        public FieldDto(int id, string label, FieldType fieldType, string unit, bool? showPrevious, string currentValue, string previousValue)
        {
            Id = id;
            Label = label;
            FieldType = fieldType;
            Unit = unit;
            ShowPrevious = showPrevious;
            CurrentValue = currentValue;
            PreviousValue = previousValue;
        }

        public int Id { get; }
        public string Label { get; }
        public FieldType FieldType { get; }
        public string Unit { get; }
        public bool? ShowPrevious { get; }
        public string CurrentValue { get; }
        public string PreviousValue { get; }
    }
}
