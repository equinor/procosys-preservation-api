using Equinor.Procosys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UploadWithOverwriteAttachmentDtoValidator : UploadBaseDtoValidator<UploadWithOverwriteAttachmentDto>
    {
        public UploadWithOverwriteAttachmentDtoValidator(IOptionsMonitor<AttachmentOptions> options) : base(options)
        {
        }
    }
}
