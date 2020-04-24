using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload
{
    public class UploadTagAttachmentCommand : IRequest<Result<int>>, ITagCommandRequest
    {
        public UploadTagAttachmentCommand(int tagId, string title, string fileName, bool overwriteIfExists)
        {
            TagId = tagId;
            Title = title;
            FileName = fileName;
            OverwriteIfExists = overwriteIfExists;
        }
        public int TagId { get; }
        public string Title { get; }
        public string FileName { get; }
        public bool OverwriteIfExists { get; }
    }
}
