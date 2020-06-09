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
            EventType eventType,
            string description
            ) : base(plant)
        {
            Description = description;
            EventType = eventType;
        }

        public string Description { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public EventType EventType { get; private set; }

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
