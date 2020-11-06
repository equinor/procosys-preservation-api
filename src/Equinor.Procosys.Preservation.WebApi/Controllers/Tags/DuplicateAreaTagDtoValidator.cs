using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class DuplicateAreaTagDtoValidator : AbstractValidator<DuplicateAreaTagDto>
    {
        public DuplicateAreaTagDtoValidator()
        {
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.DisciplineCode)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Tag.DisciplineCodeLengthMax);

            RuleFor(x => x.AreaCode)
                .MaximumLength(Tag.AreaCodeLengthMax);

            RuleFor(x => x.TagNoSuffix)
                .Must(NotContainWhiteSpace)
                .WithMessage("Tag number suffix can not contain whitespace");

            RuleFor(x => x.Description)
                .MaximumLength(Tag.DescriptionLengthMax);

            RuleFor(x => x.Remark)
                .MaximumLength(Tag.RemarkLengthMax);

            RuleFor(x => x.StorageArea)
                .MaximumLength(Tag.StorageAreaLengthMax);

            bool NotContainWhiteSpace(string suffix)
            {
                if (string.IsNullOrEmpty(suffix))
                {
                    return true;
                }

                return !suffix.Any(char.IsWhiteSpace);
            }
        }
    }
}
