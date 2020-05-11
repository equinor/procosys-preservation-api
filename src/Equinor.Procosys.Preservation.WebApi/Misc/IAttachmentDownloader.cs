using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public interface IAttachmentDownloader
    {
        Task<FileStreamResult> GetStream(string tagAttachmentPath);
    }
}
