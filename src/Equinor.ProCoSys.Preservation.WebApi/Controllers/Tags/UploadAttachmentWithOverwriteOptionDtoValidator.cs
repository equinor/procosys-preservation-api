using Equinor.ProCoSys.Preservation.BlobStorage;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentWithOverwriteOptionDtoValidator : UploadBaseDtoValidator<UploadAttachmentWithOverwriteOptionDto>
    {
        public UploadAttachmentWithOverwriteOptionDtoValidator(IOptionsMonitor<BlobStorageOptions> options) : base(options)
        {
        }
    }
}
