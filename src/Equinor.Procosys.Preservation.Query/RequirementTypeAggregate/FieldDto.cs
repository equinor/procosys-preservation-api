namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class FieldDto
    {
        public FieldDto(int id, string label, string unit, bool isVoided, bool showPrevious, int sortKey)
        {
            Id = id;
            Label = label;
            Unit = unit;
            IsVoided = isVoided;
            ShowPrevious = showPrevious;
            SortKey = sortKey;
        }

        public int Id { get; }
        public string Label { get; }
        public string Unit { get; }
        public bool IsVoided { get; }
        public bool ShowPrevious { get; private set; }
        public int SortKey { get; }
    }
}
