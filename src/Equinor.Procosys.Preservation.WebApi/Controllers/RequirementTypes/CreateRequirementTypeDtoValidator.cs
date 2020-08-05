using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class CreateRequirementTypeDtoValidator : AbstractValidator<CreateRequirementTypeDto>
    {
        public CreateRequirementTypeDtoValidator()
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
