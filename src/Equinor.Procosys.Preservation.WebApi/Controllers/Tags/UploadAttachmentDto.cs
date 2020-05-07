using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentDto
    {
        public IFormFile File { get; set; }
        public bool OverwriteIfExists { get; set; }
    }
}
