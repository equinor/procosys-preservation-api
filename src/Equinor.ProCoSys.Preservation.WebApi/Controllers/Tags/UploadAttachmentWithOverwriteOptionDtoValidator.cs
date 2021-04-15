using Equinor.ProCoSys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentWithOverwriteOptionDtoValidator : UploadBaseDtoValidator<UploadAttachmentWithOverwriteOptionDto>
    {
        public UploadAttachmentWithOverwriteOptionDtoValidator(IOptionsMonitor<AttachmentOptions> options) : base(options)
        {
        }
    }
}
