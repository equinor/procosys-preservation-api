using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public abstract class AttachmentBase : PlantEntityBase, ICreationAuditable, IModificationAuditable
    {
        public const int TitleLengthMax = 255;

        protected AttachmentBase()
            : base(null)
        {
        }

        protected AttachmentBase(string plant, string title, Guid blobStorageId)
            : base(plant)
        {
            Title = title;
            BlobStorageId = blobStorageId;
        }

        public string Title { get; private set; }
        public Guid BlobStorageId { get; private set; }
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
