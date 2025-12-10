using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.Domain.Events;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class PreservationPeriod : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
    {
        private readonly List<FieldValue> _fieldValues = new();

        public const int CommentLengthMax = 2048;

        protected PreservationPeriod()
            : base(null)
        {
        }

        public PreservationPeriod(string plant, int intervalWeeks, PreservationPeriodStatus status)
            : base(plant)
        {
            Guid = Guid.NewGuid();

            if (status != PreservationPeriodStatus.NeedsUserInput && status != PreservationPeriodStatus.ReadyToBePreserved)
            {
                throw new ArgumentException($"{status} is an illegal initial status for a {nameof(PreservationPeriod)}");
            }
            SetDueTimeUtc(intervalWeeks);
            Status = status;
        }

        public PreservationPeriodStatus Status { get; private set; }
        public DateTime DueTimeUtc { get; private set; }
        public string Comment { get; set; }
        public PreservationRecord PreservationRecord { get; private set; }
        public IReadOnlyCollection<FieldValue> FieldValues => _fieldValues.AsReadOnly();
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public int TagRequirementId { get; set; }

        public Guid Guid { get; private set; }

        public void Preserve(Person preservedBy, bool bulkPreserved)
        {
            if (PreservationRecord != null)
            {
                throw new Exception($"{nameof(PreservationPeriod)} already has a {nameof(PreservationRecord)}. Can't preserve");
            }

            if (Status != PreservationPeriodStatus.ReadyToBePreserved)
            {
                throw new Exception($"{Status} is an illegal status for {nameof(PreservationPeriod)}. Can't preserve");
            }

            Status = PreservationPeriodStatus.Preserved;
            PreservationRecord = new PreservationRecord(base.Plant, preservedBy, bulkPreserved);
        }

        public void UpdateStatus(RequirementDefinition requirementDefinition)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }

            if (Status != PreservationPeriodStatus.ReadyToBePreserved && Status != PreservationPeriodStatus.NeedsUserInput)
            {
                throw new Exception($"{Status} is an illegal status for {nameof(PreservationPeriod)} when updating status");
            }

            var fieldNeedUserInput = requirementDefinition.Fields.Where(f => f.NeedsUserInput).ToList();
            var fieldNeedUserInputIds = fieldNeedUserInput.Select(f => f.Id);
            var recordedIds = _fieldValues.Select(fv => fv.FieldId);

            if (fieldNeedUserInputIds.All(id => recordedIds.Contains(id)))
            {
                var numberFieldNeedUserInputIds = fieldNeedUserInput
                    .Where(f => f.FieldType == FieldType.Number)
                    .Select(f => f.Id)
                    .ToList();

                if (!numberFieldNeedUserInputIds.Any())
                {
                    Status = PreservationPeriodStatus.ReadyToBePreserved;
                    return;
                }

                var numberValues = _fieldValues
                    .Where(fv => numberFieldNeedUserInputIds.Contains(fv.FieldId))
                    .Select(fv => (NumberValue)fv);

                Status = numberValues.Any(nv => nv.Value.HasValue) ? PreservationPeriodStatus.ReadyToBePreserved : PreservationPeriodStatus.NeedsUserInput;
            }
            else
            {
                Status = PreservationPeriodStatus.NeedsUserInput;
            }
        }

        public void SetComment(string comment)
        {
            if (Status != PreservationPeriodStatus.ReadyToBePreserved && Status != PreservationPeriodStatus.NeedsUserInput)
            {
                throw new Exception($"{Status} is an illegal status for {nameof(PreservationPeriod)} when setting comment");
            }

            Comment = comment;
        }

        public void RecordCheckBoxValueForField(Field field, bool value)
        {
            if (field.FieldType != FieldType.CheckBox)
            {
                throw new Exception($"Can't record a {nameof(FieldType.CheckBox)} value for a {field.FieldType} field");
            }
            ValidateAndPrepareForNewRecording(field);

            // save new value ONLY if CheckBox is Checked!
            if (value)
            {
                AddFieldValue(new CheckBoxChecked(Plant, field));
            }
        }

        public void RecordNumberValueForField(Field field, double? value)
        {
            if (field.FieldType != FieldType.Number)
            {
                throw new Exception($"Can't record a {nameof(FieldType.Number)} value for a {field.FieldType} field");
            }
            ValidateAndPrepareForNewRecording(field);

            // save new value ONLY if there is a value!
            if (value.HasValue)
            {
                AddFieldValue(new NumberValue(Plant, field, value.Value));
            }
        }

        public void RecordNumberIsNaValueForField(Field field)
        {
            if (field.FieldType != FieldType.Number)
            {
                throw new Exception($"Can't record a NA {nameof(FieldType.Number)} value for a {field.FieldType} field");
            }
            ValidateAndPrepareForNewRecording(field);

            AddFieldValue(new NumberValue(Plant, field, null));
        }

        public FieldValueAttachment GetAlreadyRecordedAttachmentValueForField(Field field)
        {
            if (field.FieldType != FieldType.Attachment)
            {
                throw new Exception($"Can't record a {nameof(FieldType.Attachment)} value for a {field.FieldType} field");
            }

            if (_fieldValues.SingleOrDefault(fv => fv.FieldId == field.Id) is AttachmentValue fieldValue)
            {
                return fieldValue.FieldValueAttachment;
            }

            return null;
        }

        public void RecordAttachmentValueForField(Field field, FieldValueAttachment attachment)
        {
            if (field.FieldType != FieldType.Attachment)
            {
                throw new Exception($"Can't record a {nameof(FieldType.Attachment)} value for a {field.FieldType} field");
            }
            ValidateAndPrepareForNewRecording(field);

            // save new value ONLY if there is a value!
            if (attachment != null)
            {
                AddFieldValue(new AttachmentValue(Plant, field, attachment));
            }
        }

        public FieldValue GetFieldValue(int fieldId)
            => FieldValues.SingleOrDefault(fv => fv.FieldId == fieldId);

        public DateTime SetDueTimeUtc(int intervalWeeks)
        {
            DueTimeUtc = TimeService.UtcNow.AddWeeks(intervalWeeks);
            return DueTimeUtc;
        }

        public DateTime Reschedule(int weeks, RescheduledDirection direction)
        {
            var rescheduleWeeks = weeks;
            if (direction == RescheduledDirection.Earlier)
            {
                rescheduleWeeks *= -1;
            }
            DueTimeUtc = DueTimeUtc.AddWeeks(rescheduleWeeks);
            return DueTimeUtc;
        }

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

            AddDomainEvent(new ModifiedEvent<PreservationPeriod>(this));
        }

        private void AddFieldValue(FieldValue fieldValue)
        {
            if (fieldValue == null)
            {
                throw new ArgumentNullException(nameof(fieldValue));
            }

            _fieldValues.Add(fieldValue);
        }

        private void RemoveAnyOldFieldValue(int fieldId)
        {
            var fieldValue = _fieldValues.SingleOrDefault(fv => fv.FieldId == fieldId);
            if (fieldValue != null)
            {
                _fieldValues.Remove(fieldValue);
            }
        }

        private void ValidateAndPrepareForNewRecording(Field field)
        {
            if (field.Plant != Plant)
            {
                throw new ArgumentException($"Can't record value in {field.Plant} for item in {Plant}");
            }

            if (Status != PreservationPeriodStatus.ReadyToBePreserved &&
                Status != PreservationPeriodStatus.NeedsUserInput)
            {
                throw new Exception(
                    $"{Status} is an illegal status for {nameof(PreservationPeriod)} when recording field value");
            }

            RemoveAnyOldFieldValue(field.Id);
        }
    }
}
