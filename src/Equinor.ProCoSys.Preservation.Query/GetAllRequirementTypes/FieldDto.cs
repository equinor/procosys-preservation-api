using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Query.GetAllRequirementTypes
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
            bool? showPrevious,
            string rowVersion)
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
        public bool? ShowPrevious { get; }
        public string RowVersion { get; }
    }
}
