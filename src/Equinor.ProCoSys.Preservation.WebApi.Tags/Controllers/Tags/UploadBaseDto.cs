using Microsoft.AspNetCore.Http;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public abstract class UploadBaseDto
    {
        public IFormFile File { get; set; }
    }
}
