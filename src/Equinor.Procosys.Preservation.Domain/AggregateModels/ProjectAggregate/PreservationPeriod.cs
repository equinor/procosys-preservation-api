using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class PreservationPeriod : SchemaEntityBase, ICreationAuditable, IModificationAuditable
    {
        private readonly List<FieldValue> _fieldValues = new List<FieldValue>();

        public const int CommentLengthMax = 2048;

        protected PreservationPeriod()
            : base(null)
        {
        }
        
        public PreservationPeriod(
            string schema,
            DateTime dueTimeUtc,
            PreservationPeriodStatus status) : base(schema)
        {

            if (status != PreservationPeriodStatus.NeedsUserInput && status != PreservationPeriodStatus.ReadyToBePreserved)
            {
                throw new ArgumentException($"{status} is an illegal initial status for a {nameof(PreservationPeriod)}");
            }
            DueTimeUtc = dueTimeUtc;
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
            PreservationRecord = new PreservationRecord(base.Schema, preservedBy, bulkPreserved);
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

            var fieldNeedUserInputIds = requirementDefinition.Fields.Where(f => f.NeedsUserInput).Select(f => f.Id);
            var recordedIds = _fieldValues.Select(fv => fv.FieldId);

            if (fieldNeedUserInputIds.All(id => recordedIds.Contains(id)))
            {
                var numberFieldIds = requirementDefinition
                    .Fields
                    .Where(f => f.FieldType == FieldType.Number)
                    .Select(f => f.Id)
                    .ToList();
                if (!numberFieldIds.Any())
                {
                    Status = PreservationPeriodStatus.ReadyToBePreserved;
                    return;
                }

                var numberValues = _fieldValues
                    .Where(fv => numberFieldIds.Contains(fv.FieldId))
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

        public void RecordValueForField(Field field, string value)
        {
            if (Status != PreservationPeriodStatus.ReadyToBePreserved && Status != PreservationPeriodStatus.NeedsUserInput)
            {
                throw new Exception($"{Status} is an illegal status for {nameof(PreservationPeriod)} when recording field value");
            }

            RemoveAnyOldFieldValue(field.Id);

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            switch (field.FieldType)
            {
                case FieldType.Number:
                    RecordNumberValueForField(field, value);
                    break;
                case FieldType.CheckBox:
                    RecordCheckBoxValueForField(field, value);
                    break;
                case FieldType.Attachment:
                    // todo
                    break;
                default:
                    throw new Exception($"Can't record value for {field.FieldType}");
            }
        }
        
        public FieldValue GetFieldValue(int fieldId)
            => FieldValues.SingleOrDefault(fv => fv.FieldId == fieldId);

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            ModifiedById = modifiedBy.Id;
        }

        private void RecordCheckBoxValueForField(Field field, string value)
        {
            if (!CheckBoxChecked.IsValidValue(value, out var isChecked))
            {
                throw new ArgumentException($"Value {value} is not legal value for a {nameof(Field)} of type {field.FieldType}");
            }
            
            if (field.Schema != Schema)
            {
                throw new ArgumentException($"Can't relate item in {field.Schema} to item in {Schema}");
            }

            // save new value ONLY if CheckBox is Checked!
            if (!isChecked)
            {
                return;
            }

            AddFieldValue(new CheckBoxChecked(Schema, field));
        }

        private void RecordNumberValueForField(Field field, string value)
            => AddFieldValue(new NumberValue(Schema, field, value));
        
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
    }
}
