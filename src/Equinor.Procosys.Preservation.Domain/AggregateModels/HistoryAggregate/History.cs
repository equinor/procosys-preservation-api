using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate
{
    public class History : PlantEntityBase, IAggregateRoot, ICreationAuditable
    {
        public const int DescriptionLengthMax = 1024;

        protected History()
            : base(null)
        {
        }

        public History(
            string plant,
            string description,
            Guid objectGuid,
            ObjectType objectType,
            EventType eventType
        ) : base(plant)
        {
            Description = description;
            ObjectGuid = objectGuid;
            ObjectType = objectType;
            EventType = eventType;
        }

        public History(
            string plant,
            string description,
            Guid objectGuid,
            PreservationRecord preservationRecord
        ) : this(plant, description, objectGuid, ObjectType.Tag, EventType.RequirementPreserved)
        {
            if (preservationRecord == null)
            {
                throw new ArgumentNullException(nameof(preservationRecord));
            }
            if (preservationRecord.Plant != plant)
            {
                throw new ArgumentException($"Can't relate item in {preservationRecord.Plant} to item in {plant}");
            }

            PreservationRecordId = preservationRecord.Id;
        }

        public string Description { get; private set; }
        public int CreatedById { get; private set; }
        public Guid ObjectGuid { get; private set; }
        public int? PreservationRecordId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public EventType EventType { get; private set; }
        public ObjectType ObjectType { get; private set; }
        public int? DueInWeeks { get; set; }
        public Guid? PreservationRecordGuid { get; set; }

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }
    }
}
