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

            RuleFor(x => x.updatedRequirements)
                .NotNull();

            RuleFor(x => x)
                .Must(BeUniqueRequirements)
                .WithMessage("Requirement definitions must be unique!");

            RuleForEach(x => x.NewRequirements)
                .Must(NewRequirementMustHavePositiveInterval)
                .WithMessage($"{nameof(TagRequirementDto.IntervalWeeks)} must be positive");

            RuleForEach(x => x.updatedRequirements)
                .Must(UpdatedRequirementMustHavePositiveInterval)
                .WithMessage($"{nameof(TagRequirementDto.IntervalWeeks)} must be positive");


            bool BeUniqueRequirements(UpdateTagStepAndRequirementsDto dto)
            {
                var reqIds = dto.updatedRequirements.Select(u=>u.RequirementDefinitionId)
                    .Union(dto.NewRequirements.Select(n=>n.RequirementDefinitionId)).ToList();
                return reqIds.Distinct().Count() == reqIds.Count;
            }
        }

        private bool UpdatedRequirementMustHavePositiveInterval(UpdatedTagRequirementDto arg) => arg.IntervalWeeks > 0;

        private bool NewRequirementMustHavePositiveInterval(TagRequirementDto arg) => arg.IntervalWeeks > 0;
    }
}
