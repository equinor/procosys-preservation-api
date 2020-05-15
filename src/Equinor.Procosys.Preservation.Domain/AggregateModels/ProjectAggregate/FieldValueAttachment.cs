using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class FieldValueAttachment : Attachment
    {
        protected FieldValueAttachment() : base()
        {
        }

        public FieldValueAttachment(string plant, Guid blobStorageId, string fileName)
            : base(plant, blobStorageId, fileName, "FieldValue")
        {
        }
    }
}
