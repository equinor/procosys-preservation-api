using Equinor.ProCoSys.BlobStorage;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class UploadAttachmentForceOverwriteDtoValidator : UploadBaseDtoValidator<UploadAttachmentForceOverwriteDto>
    {
        public UploadAttachmentForceOverwriteDtoValidator(IOptionsSnapshot<BlobStorageOptions> options) : base(options)
        {
        }
    }
}
