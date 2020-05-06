using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class TagAttachment : Attachment
    {
        protected TagAttachment() : base()
        {
        }

        public TagAttachment(string plant, string fileName, Guid blobStorageId, string title)
            : base(plant, fileName, blobStorageId, title)
        {
        }
    }
}
