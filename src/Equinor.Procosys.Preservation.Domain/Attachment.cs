using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain
{
    public abstract class Attachment : PlantEntityBase, ICreationAuditable, IModificationAuditable
    {
        public const int TitleLengthMax = 255;
        public const int FileNameLengthMax = 255;

        protected Attachment()
            : base(null)
        {
        }

        protected Attachment(string plant, Guid blobStorageId, string title, string fileName)
            : base(plant)
        {
            BlobStorageId = blobStorageId;
            Title = title;
            FileName = fileName;
        }

        public Guid BlobStorageId { get; private set; }
        public string Title { get; set; }
        public string FileName { get; private set; }
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
