using Microsoft.AspNetCore.Http;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public abstract class UploadBaseDto
    {
        public IFormFile File { get; set; }
    }
}
