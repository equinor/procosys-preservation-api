using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.BlobStorage
{
    public class AzureBlobService : IBlobStorage
    {
        private class ResourceTypes
        {
            public const string BLOB = "b";
            public const string CONTAINER = "c";
        }
        private readonly string _connectionString;
        private readonly string _endpoint;
        private readonly string _accountName;
        private readonly string _accountKey;

        public AzureBlobService(IOptions<BlobStorageOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
            _accountName = Regex.Match(_connectionString, @"AccountName=(.+?)(;|\z)", RegexOptions.Singleline).Groups[1].Value;
            _accountKey = Regex.Match(_connectionString, @"AccountKey=(.+?)(;|\z)", RegexOptions.Singleline).Groups[1].Value;
            _endpoint = "blob." + Regex.Match(_connectionString, @"EndpointSuffix=(.+?)(;|\z)", RegexOptions.Singleline).Groups[1].Value;
        }

        public async Task<bool> DownloadAsync(string path, Stream destination, CancellationToken cancellationToken = default)
        {
            var container = GetContainerName(path);
            var blobPath = GetPathWithoutContainer(path);
            var client = new BlobClient(_connectionString, container, blobPath);
            var res = await client.DownloadToAsync(destination, cancellationToken);
            return res.Status > 199 && res.Status < 300;
        }

        public async Task UploadAsync(string path, Stream content, bool overWrite = false, CancellationToken cancellationToken = default)
        {
            var container = GetContainerName(path);
            var blobPath = GetPathWithoutContainer(path);
            var client = new BlobClient(_connectionString, container, blobPath);
            await client.UploadAsync(content, overWrite, cancellationToken);
        }

        public async Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            var container = GetContainerName(path);
            var blobPath = GetPathWithoutContainer(path);
            var client = new BlobClient(_connectionString, container, blobPath);
            var res = await client.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, cancellationToken);
            return res.Value;
        }

        public async Task<List<string>> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            var client = new BlobContainerClient(_connectionString, path);
            var blobNames = new List<string>();
            await foreach (var blob in client.GetBlobsAsync(BlobTraits.None, BlobStates.None, null, cancellationToken))
            {
                blobNames.Add(blob.Name);
            }
            return blobNames;
        }

        public Uri GetDownloadSasUri(string path, DateTimeOffset startsOn, DateTimeOffset expiresOn)
        {
            var container = GetContainerName(path);
            var blobPath = GetPathWithoutContainer(path);
            var sasToken = GetSasToken(container, blobPath, ResourceTypes.BLOB, BlobAccountSasPermissions.Read, startsOn, expiresOn);
            var fullUri = new UriBuilder
            {
                Scheme = "https",
                Host = string.Format($"{_accountName}.{_endpoint}"),
                Path = path,
                Query = sasToken
            };
            return fullUri.Uri;
        }

        public Uri GetUploadSasUri(string path, DateTimeOffset startsOn, DateTimeOffset expiresOn)
        {
            var container = GetContainerName(path);
            var blobPath = GetPathWithoutContainer(path);
            var sasToken = GetSasToken(container, blobPath, ResourceTypes.BLOB, BlobAccountSasPermissions.Create | BlobAccountSasPermissions.Write, startsOn, expiresOn);
            var fullUri = new UriBuilder
            {
                Scheme = "https",
                Host = string.Format($"{_accountName}.{_endpoint}"),
                Path = path,
                Query = sasToken
            };
            return fullUri.Uri;
        }

        public Uri GetDeleteSasUri(string path, DateTimeOffset startsOn, DateTimeOffset expiresOn)
        {
            var container = GetContainerName(path);
            var blobPath = GetPathWithoutContainer(path);
            var sasToken = GetSasToken(container, blobPath, ResourceTypes.BLOB, BlobAccountSasPermissions.Delete, startsOn, expiresOn);
            var fullUri = new UriBuilder
            {
                Scheme = "https",
                Host = string.Format($"{_accountName}.{_endpoint}"),
                Path = path,
                Query = sasToken
            };
            return fullUri.Uri;
        }

        public Uri GetListSasUri(string path, DateTimeOffset startsOn, DateTimeOffset expiresOn)
        {
            var containerName = GetContainerName(path);
            var sasToken = GetSasToken(containerName, string.Empty, ResourceTypes.CONTAINER, BlobAccountSasPermissions.List, startsOn, expiresOn);
            var fullUri = new UriBuilder
            {
                Scheme = "https",
                Host = string.Format($"{_accountName}.{_endpoint}"),
                Path = containerName,
                Query = $"restype=container&comp=list&{sasToken}"
            };
            return fullUri.Uri;
        }

        private string GetSasToken(string containerName, string blobName, string resourceType, BlobAccountSasPermissions permissions, DateTimeOffset startsOn, DateTimeOffset expiresOn)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = resourceType,
                StartsOn = startsOn,
                ExpiresOn = expiresOn
            };
            sasBuilder.SetPermissions(permissions);
            return sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_accountName, _accountKey)).ToString();
        }

        private static string GetContainerName(string path) =>
            path.Split('/').First();

        private static string GetPathWithoutContainer(string path) =>
            path.Remove(0, GetContainerName(path).Length + 1);
    }
}
