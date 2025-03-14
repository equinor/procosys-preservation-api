﻿using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class Field : PlantEntityBase, ICreationAuditable, IModificationAuditable, IVoidable, IHaveGuid
    {
        public const int LabelLengthMax = 255;
        public const int UnitLengthMax = 32;
        
        protected Field() : base(null)
        {
        }

        public Field(
            string plant,
            string label,
            FieldType fieldType,
            int sortKey,
            string unit = null,
            bool? showPrevious = null)
            : base(plant)
        {
            Guid = Guid.NewGuid();

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

        public string Label { get; set; }
        public string Unit { get; set; }
        public virtual bool IsVoided { get; set; }
        public bool? ShowPrevious { get; set; }
        public int SortKey { get; set; }
        public FieldType FieldType { get; private set; }
        public bool NeedsUserInput => !IsVoided && FieldType.NeedsUserInput();
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }
        public Guid Guid { get; private set; }

        public override string ToString() => Label;

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;
        }
    }
}
