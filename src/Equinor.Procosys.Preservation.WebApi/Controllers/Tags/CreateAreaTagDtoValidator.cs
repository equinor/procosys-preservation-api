using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateAreaTagDtoValidator : AbstractValidator<CreateAreaTagDto>
    {
        public CreateAreaTagDtoValidator()
        {
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.ProjectName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Project.NameLengthMax);

            RuleFor(x => x.DisciplineCode)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Tag.DisciplineCodeLengthMax);

            RuleFor(x => x.AreaCode)
                .MaximumLength(Tag.AreaCodeLengthMax);

            RuleFor(x => x.TagNoSuffix)
                .Must(NotContainWhiteSpace)
                .WithMessage("Tag number suffix can not contain whitespace");

            RuleForEach(x => x.Requirements)
                .Must(RequirementMustHavePositiveInterval)
                .WithMessage($"{nameof(TagRequirementDto.IntervalWeeks)} must be positive");

            RuleFor(x => x.Description)
                .MaximumLength(Tag.DescriptionLengthMax);

            RuleFor(x => x.Remark)
                .MaximumLength(Tag.RemarkLengthMax);

            bool NotContainWhiteSpace(string suffix)
            {
                if (string.IsNullOrEmpty(suffix))
                {
                    return true;
                }

                return !suffix.Any(char.IsWhiteSpace);
            }

            bool RequirementMustHavePositiveInterval(TagRequirementDto dto) => dto.IntervalWeeks > 0;
        }
    }
}
