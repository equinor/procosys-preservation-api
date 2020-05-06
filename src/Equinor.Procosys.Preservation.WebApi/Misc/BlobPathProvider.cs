using Equinor.Procosys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class BlobPathProvider : IBlobPathProvider
    {
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

        public BlobPathProvider(IOptionsMonitor<AttachmentOptions> attachmentOptions)
            => _attachmentOptions = attachmentOptions;

        public string CreatePathForAttachment(string folderName, Attachment attachment)
            => $"{_attachmentOptions.CurrentValue.BlobContainer}/{folderName}/{attachment.BlobStorageId.ToString()}/{attachment.FileName}";
    }
}
