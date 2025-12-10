using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class PreservationRecord : PlantEntityBase, ICreationAuditable, IHaveGuid
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
            Guid = Guid.NewGuid();
        }
        // private setters needed for Entity Framework
        public DateTime PreservedAtUtc { get; private set; }
        public int PreservedById { get; private set; }
        public bool BulkPreserved { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public Guid Guid { get; private set; }

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
