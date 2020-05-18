using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.DeleteAttachment
{
    public class DeleteFieldValueAttachmentCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public DeleteFieldValueAttachmentCommand(int tagId, int requirementId, int fieldId)
        {
            TagId = tagId;
            RequirementId = requirementId;
            FieldId = fieldId;
        }

        public int TagId { get; }
        public int RequirementId { get; }
        public int FieldId { get; }
    }
}
