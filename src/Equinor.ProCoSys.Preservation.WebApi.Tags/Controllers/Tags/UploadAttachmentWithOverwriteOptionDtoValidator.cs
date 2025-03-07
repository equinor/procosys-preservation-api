using Equinor.ProCoSys.BlobStorage;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class UploadAttachmentWithOverwriteOptionDtoValidator : UploadBaseDtoValidator<UploadAttachmentWithOverwriteOptionDto>
    {
        public UploadAttachmentWithOverwriteOptionDtoValidator(IOptionsSnapshot<BlobStorageOptions> options) : base(options)
        {
        }
    }
}
