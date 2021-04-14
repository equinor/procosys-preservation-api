using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Query.GetTagRequirements
{
    public class AttachmentDetailsDto
    {
        public AttachmentDetailsDto(AttachmentValue fieldValue)
        {
            if (fieldValue == null)
            {
                throw new ArgumentNullException(nameof(fieldValue));
            }
            if (fieldValue.FieldValueAttachment == null)
            {
                throw new ArgumentNullException(nameof(fieldValue.FieldValueAttachment));
            }

            Id = fieldValue.FieldValueAttachment.Id;
            FileName = fieldValue.FieldValueAttachment.FileName;
        }

        public int Id { get; }
        public string FileName { get; }
    }
}
