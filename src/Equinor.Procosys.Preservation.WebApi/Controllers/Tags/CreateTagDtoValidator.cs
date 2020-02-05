using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(x => x).NotNull();

            RuleFor(x => x.TagNos)
                .NotNull();

            RuleForEach(x => x.TagNos)
                .NotEmpty()
                .MaximumLength(Tag.TagNoLengthMax);

            RuleForEach(x => x.Requirements)
                .Must(RequirementMustHavePositiveInterval)
                .WithMessage($"{nameof(TagRequirementDto.IntervalWeeks)} must be positive");
            
            bool RequirementMustHavePositiveInterval(TagRequirementDto dto) => dto.IntervalWeeks > 0;
        }
    }
}
