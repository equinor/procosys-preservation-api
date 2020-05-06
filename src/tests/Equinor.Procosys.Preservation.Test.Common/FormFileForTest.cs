using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.Test.Common
{
    public class FormFileForTest : IFormFile
    {
        public FormFileForTest(string fileName)
        {
            Name = fileName;
            FileName = fileName;
            Length = 10;
            ContentDisposition = "CD";
            ContentType = "CT";
            Headers = null;
        }

        public virtual Stream OpenReadStream() => null;

        public virtual void CopyTo(Stream target) {}

        public virtual Task CopyToAsync(Stream target, CancellationToken cancellationToken = new CancellationToken()) => Task.CompletedTask;

        public string ContentType { get; }
        public string ContentDisposition { get; }
        public IHeaderDictionary Headers { get; }
        public long Length { get; }
        public string Name { get; }
        public string FileName { get; }
        public virtual int Type { get; set; }
    }
}
