using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(x => x.TagNo)
                .NotEmpty()
                .MaximumLength(Tag.TagNoLengthMax);

            RuleFor(x => x.ProjectName)
                .NotEmpty()
                .MaximumLength(Tag.ProjectNameLengthMax);

            RuleForEach(x => x.Requirements)
                .Must(RequirementMustHavePositiveInterval)
                .WithMessage($"{nameof(TagRequirementDto.IntervalWeeks)} must be positive");
            
            bool RequirementMustHavePositiveInterval(TagRequirementDto dto) => dto.IntervalWeeks > 0;
        }
    }
}
