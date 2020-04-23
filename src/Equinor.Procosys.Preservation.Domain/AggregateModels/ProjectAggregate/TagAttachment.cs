using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class TagAttachment : AttachmentBase
    {
        public TagAttachment(string plant, string title, Guid blobStorageId)
            : base(plant, title, blobStorageId)
        {
        }
    }
}
