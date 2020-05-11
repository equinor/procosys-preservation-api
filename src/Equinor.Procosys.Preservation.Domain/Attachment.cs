using System;
using System.IO;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain
{
    public abstract class Attachment : PlantEntityBase, ICreationAuditable, IModificationAuditable
    {
        public const int FileNameLengthMax = 255;
        public const int PathLengthMax = 1024;

        protected Attachment()
            : base(null)
        {
        }

        protected Attachment(string plant, Guid blobStorageId, string fileName, string parentType)
            : base(plant)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            if (string.IsNullOrEmpty(parentType))
            {
                throw new ArgumentNullException(nameof(parentType));
            }

            FileName = fileName;
            BlobPath = Path.Combine(plant.Substring(4), parentType, blobStorageId.ToString()).Replace("\\", "/");;
        }

        public string FileName { get; private set; }
        public string BlobPath { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        // todo unit test
        public string GetFullBlobPath(string blobContainer)
            => Path.Combine(blobContainer, BlobPath, FileName).Replace("\\", "/");

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
