using System.IO;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload
{
    public class UploadTagAttachmentCommand : UploadAttachmentCommand, IRequest<Result<int>>, ITagCommandRequest
    {
        public UploadTagAttachmentCommand(int tagId, string fileName, bool overwriteIfExists, Stream content)
            : base(content)
        {
            TagId = tagId;
            FileName = fileName;
            OverwriteIfExists = overwriteIfExists;
        }

        public int TagId { get; }
        public string FileName { get; }
        public bool OverwriteIfExists { get; }
    }
}
