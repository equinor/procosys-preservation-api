using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.BlobStorage
{
    public interface IBlobStorage
    {
        Task<bool> DownloadAsync(string path, Stream destination, CancellationToken cancellationToken = default);
        Task UploadAsync(string path, Stream content, bool overWrite = false, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default);
        Task<List<string>> ListAsync(string path, CancellationToken cancellationToken = default);

        Uri GetDownloadSasUri(string path, DateTimeOffset startsOn, DateTimeOffset expiresOn);
        Uri GetUploadSasUri(string path, DateTimeOffset startsOn, DateTimeOffset expiresOn);
        Uri GetDeleteSasUri(string path, DateTimeOffset startsOn, DateTimeOffset expiresOn);
        Uri GetListSasUri(string path, DateTimeOffset startsOn, DateTimeOffset expiresOn);
    }
}
