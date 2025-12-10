using System;
using System.IO;
using System.Text.Json.Serialization;

namespace Equinor.ProCoSys.Preservation.Command
{
    public abstract class UploadAttachmentCommand : IDisposable
    {
        private bool _isDisposed;

        protected UploadAttachmentCommand(Stream content)
            => Content = content ?? throw new ArgumentNullException(nameof(content));

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
