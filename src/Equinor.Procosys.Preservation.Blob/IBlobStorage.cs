using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Blob
{
    public interface IBlobStorage
    {
        Task<bool> DownloadAsync(string containerName, string blobName, Stream destination, CancellationToken cancellationToken = default);
        Task UploadAsync(string containerName, string blobName, Stream content, bool overWrite = false, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
        Task<List<string>> ListAsync(string containerName, CancellationToken cancellationToken = default);

        Uri GetDownloadSasUri(string containerName, string blobName, DateTimeOffset startsOn, DateTimeOffset expiresOn);
        Uri GetUploadSasUri(string containerName, string blobName, DateTimeOffset startsOn, DateTimeOffset expiresOn);
        Uri GetDeleteSasUri(string containerName, string blobName, DateTimeOffset startsOn, DateTimeOffset expiresOn);
        Uri GetListSasUri(string containerName, DateTimeOffset startsOn, DateTimeOffset expiresOn);
    }
}
