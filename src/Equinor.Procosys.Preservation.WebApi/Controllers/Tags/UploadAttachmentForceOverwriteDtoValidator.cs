using Equinor.ProCoSys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentForceOverwriteDtoValidator : UploadBaseDtoValidator<UploadAttachmentForceOverwriteDto>
    {
        public UploadAttachmentForceOverwriteDtoValidator(IOptionsMonitor<AttachmentOptions> options) : base(options)
        {
        }
    }
}
