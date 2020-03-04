using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class Field : SchemaEntityBase, ICreationAuditable, IModificationAuditable
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
        public bool NeedsUserInput =>
            FieldType == FieldType.Number ||
            FieldType == FieldType.Attachment ||
            FieldType == FieldType.CheckBox;

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public override string ToString() => Label;

        public void SetCreated(DateTime createdAtUtc, Person createdBy)
        {
            if (createdAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(createdAtUtc)} is not UTC");
            }

            CreatedAtUtc = createdAtUtc;
            CreatedById = createdBy.Id;
        }

        public void SetModified(DateTime modifiedAtUtc, Person modifiedBy)
        {
            if (modifiedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(modifiedAtUtc)} is not UTC");
            }

            ModifiedAtUtc = modifiedAtUtc;
            ModifiedById = modifiedBy.Id;
        }
    }
}
