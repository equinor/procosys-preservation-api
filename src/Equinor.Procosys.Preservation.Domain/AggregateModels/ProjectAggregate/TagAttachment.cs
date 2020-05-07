using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class TagAttachment : Attachment
    {
        protected TagAttachment() : base()
        {
        }

        public TagAttachment(string plant, string fileName, Guid blobStorageId)
            : base(plant, fileName, blobStorageId)
        {
        }

        public override string BlobPath => $"{Plant.Substring(4)}/Tag/{BlobStorageId.ToString()}/{FileName}";
    }
}
