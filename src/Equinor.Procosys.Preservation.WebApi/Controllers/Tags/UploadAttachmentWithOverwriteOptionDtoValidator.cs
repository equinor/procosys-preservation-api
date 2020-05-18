using Equinor.Procosys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentWithOverwriteOptionDtoValidator : UploadBaseDtoValidator<UploadAttachmentWithOverwriteOptionDto>
    {
        public UploadAttachmentWithOverwriteOptionDtoValidator(IOptionsMonitor<AttachmentOptions> options) : base(options)
        {
        }
    }
}
