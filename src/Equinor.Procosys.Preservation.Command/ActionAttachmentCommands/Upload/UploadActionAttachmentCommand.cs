using System.IO;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Upload
{
    public class UploadActionAttachmentCommand : UploadAttachmentCommand, IRequest<Result<int>>, ITagCommandRequest
    {
        public UploadActionAttachmentCommand(int tagId, int actionId, string fileName, bool overwriteIfExists, Stream content)
            : base(content)
        {
            TagId = tagId;
            ActionId = actionId;
            FileName = fileName;
            OverwriteIfExists = overwriteIfExists;
        }

        public int TagId { get; }
        public int ActionId { get; }
        public string FileName { get; }
        public bool OverwriteIfExists { get; }
    }
}
