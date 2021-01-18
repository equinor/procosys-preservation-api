using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;
using Equinor.Procosys.Preservation.Domain.Time;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class PreservationRecord : PlantEntityBase, ICreationAuditable
    {
        protected PreservationRecord()
            : base(null)
        {
        }
        
        public PreservationRecord(string plant, Person preservedBy, bool bulkPreserved)
            : base(plant)
        {
            if (preservedBy == null)
            {
                throw new ArgumentNullException(nameof(preservedBy));
            }
            PreservedAtUtc = TimeService.UtcNow;
            PreservedById = preservedBy.Id;
            BulkPreserved = bulkPreserved;
            ObjectGuid = Guid.NewGuid();
        }

        // Todo Rename to PreservationRecordGuid
        public Guid ObjectGuid { get; private set; }
        public DateTime PreservedAtUtc { get; private set; }
        public int PreservedById { get; private set; }
        public bool BulkPreserved { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }

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
