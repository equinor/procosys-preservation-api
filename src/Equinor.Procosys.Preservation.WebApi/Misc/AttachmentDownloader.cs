using System;
using System.IO;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using HeyRed.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class AttachmentDownloader : IAttachmentDownloader
    {
        private readonly IBlobStorage _blobStorage;

        public AttachmentDownloader(IBlobStorage blobStorage) => _blobStorage = blobStorage;

        public async Task<FileStreamResult> GetStream(string tagAttachmentPath)
        {
            // don't dispose
            var stream = new MemoryStream();

            var fileName = Path.GetFileName(tagAttachmentPath);

            if (await _blobStorage.DownloadAsync(tagAttachmentPath, stream))
            {
                stream.Position = 0;

                return new FileStreamResult(stream, MimeTypesMap.GetMimeType(fileName))
                {
                    FileDownloadName = fileName
                };
            }

            throw new Exception($"Not able to download {tagAttachmentPath}");
        }
    }
}
