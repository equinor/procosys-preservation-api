using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class UpdateRequirementTypeDtoValidator : AbstractValidator<UpdateRequirementTypeDto>
    {
        public UpdateRequirementTypeDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(RequirementType.TitleLengthMax);
            RuleFor(x => x.Code)
                .MaximumLength(RequirementType.CodeLengthMax);
            RuleFor(x => x.Icon).NotNull();
            RuleFor(x => x.SortKey).NotNull();
            RuleFor(x => x.SortKey)
                .Must(BePositive)
                .WithMessage("Sort key must be positive");

            bool BePositive(int arg) => arg > 0;
        }
    }
}
