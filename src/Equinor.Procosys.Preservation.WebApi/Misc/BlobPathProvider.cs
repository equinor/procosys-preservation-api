using System.IO;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class BlobPathProvider : IBlobPathProvider
    {
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

        public BlobPathProvider(IOptionsMonitor<AttachmentOptions> attachmentOptions)
            => _attachmentOptions = attachmentOptions;

        public string CreateFullPathForAttachment(Attachment attachment)
            => Path.Combine(_attachmentOptions.CurrentValue.BlobContainer, attachment.BlobPath, attachment.FileName).Replace("\\", "/");
    }
}
