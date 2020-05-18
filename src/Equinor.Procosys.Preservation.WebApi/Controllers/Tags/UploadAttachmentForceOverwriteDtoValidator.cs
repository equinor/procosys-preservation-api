using Equinor.Procosys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentForceOverwriteDtoValidator : UploadBaseDtoValidator<UploadAttachmentForceOverwriteDto>
    {
        public UploadAttachmentForceOverwriteDtoValidator(IOptionsMonitor<AttachmentOptions> options) : base(options)
        {
        }
    }
}
