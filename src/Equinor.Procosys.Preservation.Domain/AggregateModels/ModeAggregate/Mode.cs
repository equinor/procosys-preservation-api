using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;
using Equinor.Procosys.Preservation.Domain.Time;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate
{
    public class Mode : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable
    {
        public const int TitleLengthMin = 3;
        public const int TitleLengthMax = 255;

        protected Mode()
            : base(null)
        {
        }

        public Mode(string plant, string title, bool forSupplier) : base(plant)
        {
            Title = title;
            ForSupplier = forSupplier;
        }

        public string Title { get; set; }
        public bool IsVoided { get; private set; }
        public bool ForSupplier { get; set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

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
