using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class AttachmentDto
    {
        public AttachmentDto(AttachmentValue fieldValue)
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
