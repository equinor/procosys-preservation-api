namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class Field : SchemaEntityBase
    {
        public const int LabelLengthMax = 255;
        public const int UnitLengthMax = 32;
        
        protected Field() : base(null)
        {
        }

        public Field(string schema,
            string label,
            string unit,
            bool showPrevious,
            FieldType fieldType,
            int sortKey)
            : base(schema)
        {
            Label = label;
            Unit = unit;
            ShowPrevious = showPrevious;
            FieldType = fieldType;
            SortKey = sortKey;
        }

        public string Label { get; private set; }
        public string Unit { get; private set; }
        public bool IsVoided { get; private set; }
        public bool ShowPrevious { get; private set; }
        public int SortKey { get; private set; }
        public FieldType FieldType { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
    }
}
