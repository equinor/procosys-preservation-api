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

            RuleForEach(x => x.Requirements)
                .Must(RequirementMustHavePositiveInterval)
                .WithMessage($"{nameof(TagRequirementDto.IntervalWeeks)} must be positive");
            
            bool RequirementMustHavePositiveInterval(TagRequirementDto dto) => dto.IntervalWeeks > 0;
        }
    }
}
