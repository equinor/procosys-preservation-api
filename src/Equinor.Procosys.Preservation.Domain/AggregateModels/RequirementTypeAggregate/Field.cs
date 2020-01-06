namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class Field : SchemaEntityBase
    {
        public const int LabelLengthMax = 255;
        public const int UnitLengthMax = 32;
        
        protected Field(string schema) : base(schema)
        {
        }

        public Field(string schema, string label, string unit,
            bool showPrevious, int sortKey)
            : base(schema)
        {
            Label = label;
            Unit = unit;
            IsVoided = false;
            ShowPrevious = showPrevious;
            SortKey = sortKey;
        }

        public string Label { get; private set; }
        public string Unit { get; private set; }
        public bool IsVoided { get; private set; }
        public bool ShowPrevious { get; private set; }
        public int SortKey { get; private set; }
    }
}
