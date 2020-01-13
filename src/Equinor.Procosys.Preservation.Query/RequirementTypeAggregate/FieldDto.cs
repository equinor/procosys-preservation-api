using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class FieldDto
    {
        public FieldDto(int id, string label, string unit, bool isVoided, bool showPrevious, FieldType fieldType, int sortKey)
        {
            Id = id;
            Label = label;
            Unit = unit;
            IsVoided = isVoided;
            ShowPrevious = showPrevious;
            FieldType = fieldType;
            SortKey = sortKey;
        }

        public int Id { get; }
        public string Label { get; }
        public string Unit { get; }
        public bool IsVoided { get; }
        public bool ShowPrevious { get; }
        public FieldType FieldType { get; }
        public int SortKey { get; }
        public bool NeedUserInput =>
            FieldType == FieldType.Number ||
            FieldType == FieldType.Attachment ||
            FieldType == FieldType.CheckBox;
    }
}
