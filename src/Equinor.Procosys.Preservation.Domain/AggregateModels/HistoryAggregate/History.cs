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
            int objectId,
            ObjectType objectType,
            EventType eventType
        ) : base(plant)
        {
            Description = description;
            ObjectId = objectId;
            ObjectType = objectType;
            EventType = eventType;
        }

        public History(
            string plant,
            string description,
            int objectId,
            PreservationRecord preservationRecord
        ) : this(plant, description, objectId, ObjectType.Tag, EventType.PreserveRequirement)
        {
            if (preservationRecord == null)
            {
                throw new ArgumentNullException(nameof(preservationRecord));
            }
            if (preservationRecord.Plant != plant)
            {
                throw new ArgumentException($"Can't relate item in {preservationRecord.Plant} to item in {plant}");
            }
        }

        public string Description { get; private set; }
        public int CreatedById { get; private set; }
        public int ObjectId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public EventType EventType { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public PreservationRecord PreservationRecord { get; private set; }

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
