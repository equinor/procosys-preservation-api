using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Common.Time;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate
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

        public string Description { get; private set; }
        public int CreatedById { get; private set; }
        public Guid ObjectGuid { get; private set; }
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
