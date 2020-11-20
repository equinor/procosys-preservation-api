using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UpdateTagStepAndRequirementsDtoValidator : AbstractValidator<UpdateTagStepAndRequirementsDto>
    {
        public UpdateTagStepAndRequirementsDtoValidator()
        {
            RuleFor(x => x).NotNull();

            When(x => x.NewRequirements != null, () =>
            {
                RuleForEach(x => x.NewRequirements)
                    .Must(NewRequirementMustHavePositiveInterval)
                    .WithMessage("Week interval must be positive!");
            });

            When(x => x.UpdatedRequirements != null, () =>
            {
                RuleForEach(x => x.UpdatedRequirements)
                    .Must(UpdatedRequirementMustHavePositiveInterval)
                    .WithMessage("Week interval must be positive!");
            });

            bool UpdatedRequirementMustHavePositiveInterval(UpdatedTagRequirementDto arg) => arg.IntervalWeeks > 0;

            bool NewRequirementMustHavePositiveInterval(TagRequirementDto arg) => arg.IntervalWeeks > 0;
        }
    }
}
