using System;
using System.IO;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload
{
    public class UploadTagAttachmentCommand : IRequest<Result<int>>, ITagCommandRequest, IDisposable
    {
        private bool _isDisposed = false;

        public UploadTagAttachmentCommand(int tagId, Stream content, string fileName, string title, bool overwriteIfExists)
        {
            TagId = tagId;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            FileName = fileName;
            Title = title;
            OverwriteIfExists = overwriteIfExists;
        }

        public int TagId { get; }
        public Stream Content { get; }
        public string Title { get; }
        public string FileName { get; }
        public bool OverwriteIfExists { get; }

        public void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;;
            }

            if (disposing)
            {
                Content.Dispose();
            }
       
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
