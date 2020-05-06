using System;
using MediatR;
using Microsoft.AspNetCore.Http;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload
{
    public class UploadTagAttachmentCommand : IRequest<Result<int>>, ITagCommandRequest
    {
        public UploadTagAttachmentCommand(int tagId, IFormFile file, string title, bool overwriteIfExists)
        {
            TagId = tagId;
            File = file ?? throw new ArgumentNullException(nameof(file));
            Title = title;
            OverwriteIfExists = overwriteIfExists;
        }
        public int TagId { get; }
        public IFormFile File { get; }
        public string Title { get; }
        public bool OverwriteIfExists { get; }
    }
}
