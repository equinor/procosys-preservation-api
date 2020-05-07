using System.IO;
using System.Linq;
using Equinor.Procosys.Preservation.Domain;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentDtoValidator : AbstractValidator<UploadAttachmentDto>
    {
        public UploadAttachmentDtoValidator(IOptionsMonitor<AttachmentOptions> options)
        {
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.File)
                .NotNull()
                .NotEmpty();
            
            RuleFor(x => x.File.FileName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Attachment.FileNameLengthMax)
                .Must(BeAValidFile)
                .WithMessage(x => $"File {x.File.FileName}is not a valid file for upload")
                .When(x => x.File != null);
            
            RuleFor(x => x.File.Length/1000)
                .LessThan(options.CurrentValue.MaxSizeKb)
                .When(x => x.File != null)
                .WithMessage($"Maximum file size is {options.CurrentValue.MaxSizeKb}kB");

            bool BeAValidFile(string fileName)
            {
                var suffix = Path.GetExtension(fileName.ToLower());
                return options.CurrentValue.ValidFileSuffixArray.Contains(suffix) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
            }
        }
    }
}
