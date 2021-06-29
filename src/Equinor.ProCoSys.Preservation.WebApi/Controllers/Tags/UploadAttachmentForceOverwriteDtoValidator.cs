using Equinor.ProCoSys.Preservation.BlobStorage;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentForceOverwriteDtoValidator : UploadBaseDtoValidator<UploadAttachmentForceOverwriteDto>
    {
        public UploadAttachmentForceOverwriteDtoValidator(IOptionsMonitor<BlobStorageOptions> options) : base(options)
        {
        }
    }
}
