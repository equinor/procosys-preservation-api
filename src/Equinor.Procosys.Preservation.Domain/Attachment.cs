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

        protected Attachment(string plant, string fileName, Guid blobStorageId, string title)
            : base(plant)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            BlobStorageId = blobStorageId;
            SetTitle(title, fileName);
            FileName = fileName;
        }

        public Guid BlobStorageId { get; private set; }
        public string Title { get; private set; }
        public string FileName { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public abstract string BlobPath { get; }

        public void SetTitle(string title, string fileName) => Title = title ?? fileName;

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
