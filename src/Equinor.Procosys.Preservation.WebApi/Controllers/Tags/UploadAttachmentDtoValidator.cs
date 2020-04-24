using Equinor.Procosys.Preservation.Domain;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentDtoValidator : AbstractValidator<UploadAttachmentDto>
    {
        public UploadAttachmentDtoValidator()
        {
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.Title)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Attachment.TitleLengthMax);

            RuleFor(x => x.FileName)
                .MaximumLength(Attachment.FileNameLengthMax);
        }
    }
}
