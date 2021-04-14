using System;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class TagAttachment : Attachment
    {
        protected TagAttachment() : base()
        {
        }

        public TagAttachment(string plant, Guid blobStorageId, string fileName)
            : base(plant, blobStorageId, fileName, "Tag")
        {
        }
    }
}
