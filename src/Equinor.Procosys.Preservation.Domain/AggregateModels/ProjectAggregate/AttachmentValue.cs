using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    // A field needing input can be of 3 types: Number, CheckBox and Attachment
    // This class represent recorded value in an Attachment field
    // I.e:
    //      If table row exists -> end user has uploaded an attachment for particular field and saved
    //      If end user delete that attachment for particular field, and save, table row will be deleted
    public class AttachmentValue : FieldValue
    {
        protected AttachmentValue()
        {
        }

        public AttachmentValue(string plant, Field field, FieldValueAttachment attachment)
            : base(plant, field)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            FieldValueAttachmentId = attachment.Id;
        }

        public int FieldValueAttachmentId { get; private set; }
    }
}
