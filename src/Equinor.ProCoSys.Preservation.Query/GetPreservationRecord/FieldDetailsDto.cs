﻿using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Query.GetTagRequirements;

namespace Equinor.ProCoSys.Preservation.Query.GetPreservationRecord
{
    public class FieldDetailsDto
    {
        public FieldDetailsDto(Field field, FieldValue currentValue, FieldValue previousValue)
        {
            Id = field.Id;
            Label = field.Label;
            FieldType = field.FieldType;
            Unit = field.Unit;
            ShowPrevious = field.ShowPrevious.HasValue && field.ShowPrevious.Value;
            CurrentValue = CreateFieldValueDto(field.FieldType, currentValue);
            if (ShowPrevious)
            {
                PreviousValue = CreateFieldValueDto(field.FieldType, previousValue);
            }
        }

        public int Id { get; }
        public string Label { get; }
        public FieldType FieldType { get; }
        public string Unit { get; }
        public bool ShowPrevious { get; }
        public object CurrentValue { get; }
        public object PreviousValue { get; }
        private object CreateFieldValueDto(FieldType fieldType, FieldValue fieldValue)
        {
            if (fieldValue == null)
            {
                return null;
            }
            switch (fieldType)
            {
                case FieldType.Number:
                    return new NumberDetailsDto(fieldValue as NumberValue);
                case FieldType.CheckBox:
                    return new CheckBoxDetailsDto();
                case FieldType.Attachment:
                    return new AttachmentDetailsDto(fieldValue as AttachmentValue);
                default:
                    return null;
            }
        }
    }
}
