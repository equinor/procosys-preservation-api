using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ActionAttachmentCommands.Delete
{
    public class DeleteActionAttachmentCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public DeleteActionAttachmentCommand(int tagId, int actionId, int attachmentId, string rowVersion)
        {
            TagId = tagId;
            ActionId = actionId;
            AttachmentId = attachmentId;
            RowVersion = rowVersion;
        }

        public int TagId { get; }
        public int ActionId { get; }
        public int AttachmentId { get; }
        public string RowVersion { get; }
    }
}
