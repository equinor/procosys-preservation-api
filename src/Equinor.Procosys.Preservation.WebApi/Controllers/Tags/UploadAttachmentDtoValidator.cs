using Equinor.Procosys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentDtoValidator : UploadBaseDtoValidator<UploadAttachmentDto>
    {
        public UploadAttachmentDtoValidator(IOptionsMonitor<AttachmentOptions> options) : base(options)
        {
        }
    }
}
