using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class FieldDto
    {
        public FieldDto(
            int id, 
            string label, 
            bool isVoided, 
            FieldType fieldType, 
            int sortKey, 
            string unit,
            string rowVersion,
            bool? showPrevious)
        {
            Id = id;
            Label = label;
            IsVoided = isVoided;
            FieldType = fieldType;
            SortKey = sortKey;
            Unit = unit;
            ShowPrevious = showPrevious;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Label { get; }
        public bool IsVoided { get; }
        public int SortKey { get; }
        public FieldType FieldType { get; }
        public string Unit { get; }
        public string RowVersion { get; }
        public bool? ShowPrevious { get; }
    }
}
