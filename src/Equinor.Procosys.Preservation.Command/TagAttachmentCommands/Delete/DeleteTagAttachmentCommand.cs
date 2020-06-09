using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Delete
{
    public class DeleteTagAttachmentCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public DeleteTagAttachmentCommand(int tagId, int attachmentId, string rowVersion, Guid currentUserOid)
        {
            TagId = tagId;
            AttachmentId = attachmentId;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }

        public int TagId { get; }
        public int AttachmentId { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
