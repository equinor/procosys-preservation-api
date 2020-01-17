using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class FieldDto
    {
        public FieldDto(int id, string label, bool isVoided, FieldType fieldType, int sortKey, bool needUserInput, string unit, bool? showPrevious)
        {
            Id = id;
            Label = label;
            IsVoided = isVoided;
            FieldType = fieldType;
            SortKey = sortKey;
            NeedUserInput = needUserInput;
            Unit = unit;
            ShowPrevious = showPrevious;
        }

        public int Id { get; }
        public string Label { get; }
        public bool IsVoided { get; }
        public int SortKey { get; }
        public FieldType FieldType { get; }
        public bool NeedUserInput { get; }
        public string Unit { get; }
        public bool? ShowPrevious { get; }
    }
}
