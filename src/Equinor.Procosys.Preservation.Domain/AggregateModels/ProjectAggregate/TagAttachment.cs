using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class TagAttachment : Attachment
    {
        protected TagAttachment() : base()
        {
        }

        public TagAttachment(string plant, Guid blobStorageId, string title, string fileName)
            : base(plant, blobStorageId, title, fileName)
        {
        }
    }
}
