using System.Linq;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UpdateTagStepAndRequirementsDtoValidator : AbstractValidator<UpdateTagStepAndRequirementsDto>
    {
        public UpdateTagStepAndRequirementsDtoValidator()
        {
            RuleFor(x => x).NotNull();

            RuleFor(x => x.NewRequirements)
                .NotNull();

            RuleFor(x => x.UpdatedRequirements)
                .NotNull();

            RuleFor(x => x)
                .Must(BeUniqueRequirements)
                .WithMessage("Requirement definitions must be unique!");

            RuleForEach(x => x.NewRequirements)
                .Must(NewRequirementMustHavePositiveInterval)
                .WithMessage("Week interval must be positive");

            RuleForEach(x => x.UpdatedRequirements)
                .Must(UpdatedRequirementMustHavePositiveInterval)
                .WithMessage("Week interval must be positive");

            bool BeUniqueRequirements(UpdateTagStepAndRequirementsDto dto)
            {
                var reqIds = dto.UpdatedRequirements.Select(u=>u.RequirementId)
                    .Union(dto.NewRequirements.Select(n=>n.RequirementDefinitionId)).ToList();
                return reqIds.Distinct().Count() == reqIds.Count;
            }

            bool UpdatedRequirementMustHavePositiveInterval(UpdatedTagRequirementDto arg) => arg.IntervalWeeks > 0;

            bool NewRequirementMustHavePositiveInterval(TagRequirementDto arg) => arg.IntervalWeeks > 0;
        }
    }
}
