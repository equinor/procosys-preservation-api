using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;
using Equinor.Procosys.Preservation.Domain.Time;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate
{
    public class Responsible : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable, IVoidable
    {
        public const int CodeLengthMax = 255;
        public const int DescriptionLengthMax = 255;

        protected Responsible()
            : base(null)
        {
        }

        public Responsible(string plant, string code, string description)
            : base(plant)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; private set; }
        public string Description { get; set; }
        public bool IsVoided { get; set; }

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

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
        }
    }
}
