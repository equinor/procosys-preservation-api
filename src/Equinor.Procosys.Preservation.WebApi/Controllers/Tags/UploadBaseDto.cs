using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public abstract class UploadBaseDto
    {
        public IFormFile File { get; set; }
    }
}
