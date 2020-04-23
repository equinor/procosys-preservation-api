using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class ActionAttachment : Attachment
    {
        protected ActionAttachment() : base()
        {
        }

        public ActionAttachment(string plant, string title, Guid blobStorageId)
            : base(plant, title, blobStorageId)
        {
        }
    }
}
