using System;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class ActionAttachment : Attachment
    {
        protected ActionAttachment() : base()
        {
        }

        public ActionAttachment(string plant, Guid blobStorageId, string fileName)
            : base(plant, blobStorageId, fileName, "Action")
        {
        }
    }
}
