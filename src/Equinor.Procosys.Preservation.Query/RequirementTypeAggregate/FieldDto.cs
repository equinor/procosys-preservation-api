using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class FieldDto
    {
        public FieldDto(int id, string label, string unit, bool isVoided, bool showPrevious, int sortKey, FieldType fieldType)
        {
            Id = id;
            Label = label;
            Unit = unit;
            IsVoided = isVoided;
            ShowPrevious = showPrevious;
            SortKey = sortKey;
            FieldType = fieldType;
        }

        public int Id { get; }
        public string Label { get; }
        public string Unit { get; }
        public bool IsVoided { get; }
        public bool ShowPrevious { get; }
        public int SortKey { get; }
        public FieldType FieldType { get; }
    }
}
