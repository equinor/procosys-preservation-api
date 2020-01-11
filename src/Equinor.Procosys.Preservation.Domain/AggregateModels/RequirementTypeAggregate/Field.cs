using System;

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
            FieldType fieldType,
            int sortKey,
            string unit = null,
            bool? showPrevious = null)
            : base(schema)
        {
            if (fieldType == FieldType.Number && string.IsNullOrEmpty(unit))
            {
                throw new ArgumentException($"{nameof(Unit)} must have value for {nameof(FieldType)} {FieldType.Number}");
            }
            if (fieldType == FieldType.Number && !showPrevious.HasValue)
            {
                throw new ArgumentException($"{nameof(ShowPrevious)} must have value for {nameof(FieldType)} {FieldType.Number}");
            }
            Label = label;
            Unit = unit;
            ShowPrevious = showPrevious;
            FieldType = fieldType;
            SortKey = sortKey;
        }

        public string Label { get; private set; }
        public string Unit { get; private set; }
        public bool IsVoided { get; private set; }
        public bool? ShowPrevious { get; private set; }
        public int SortKey { get; private set; }
        public FieldType FieldType { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
    }
}
