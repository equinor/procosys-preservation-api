using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Auth.Time;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public abstract class FieldValue : PlantEntityBase, ICreationAuditable
    {
        protected FieldValue()
            : base(null)
        {
        }

        protected FieldValue(string plant, Field field)
            : base(plant)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            FieldId = field.Id;
        }
        
        public int FieldId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }

        public FieldValueAttachment FieldValueAttachment { get; protected set; }

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
