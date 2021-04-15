using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagAttachmentCommands.Delete
{
    public class DeleteTagAttachmentCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public DeleteTagAttachmentCommand(int tagId, int attachmentId, string rowVersion)
        {
            TagId = tagId;
            AttachmentId = attachmentId;
            RowVersion = rowVersion;
        }

        public int TagId { get; }
        public int AttachmentId { get; }
        public string RowVersion { get; }
    }
}
