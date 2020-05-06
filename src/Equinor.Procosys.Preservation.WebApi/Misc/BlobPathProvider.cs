using Equinor.Procosys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class BlobPathProvider : IBlobPathProvider
    {
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

        public BlobPathProvider(IOptionsMonitor<AttachmentOptions> attachmentOptions)
            => _attachmentOptions = attachmentOptions;

        public string CreatePathForAttachment<T>(Attachment attachment) where T : class
            => $"{_attachmentOptions.CurrentValue.BlobContainer}/{typeof(T).Name}/{attachment.BlobStorageId.ToString()}/{attachment.FileName}";
    }
}
