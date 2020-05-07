using System;
using System.IO;
using System.Text.Json.Serialization;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload
{
    public class UploadTagAttachmentCommand : IRequest<Result<int>>, ITagCommandRequest, IDisposable
    {
        private bool _isDisposed;

        public UploadTagAttachmentCommand(int tagId, string fileName, bool overwriteIfExists, Stream content)
        {
            TagId = tagId;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            FileName = fileName;
            OverwriteIfExists = overwriteIfExists;
        }

        public int TagId { get; }
        public string FileName { get; }
        public bool OverwriteIfExists { get; }

        // JsonIgnore needed here so GlobalExceptionHandler do not try to serialize the Stream when reporting validation errors. 
        [JsonIgnore]
        public Stream Content { get; }

        public void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
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
