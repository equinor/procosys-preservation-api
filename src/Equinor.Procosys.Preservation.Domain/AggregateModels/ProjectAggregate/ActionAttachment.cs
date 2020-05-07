using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class ActionAttachment : Attachment
    {
        protected ActionAttachment() : base()
        {
        }

        public ActionAttachment(string plant, string fileName, Guid blobStorageId)
            : base(plant, fileName, blobStorageId)
        {
        }

        public override string BlobPath => $"{Plant.Substring(4)}/Action/{BlobStorageId.ToString()}/{FileName}";
    }
}
