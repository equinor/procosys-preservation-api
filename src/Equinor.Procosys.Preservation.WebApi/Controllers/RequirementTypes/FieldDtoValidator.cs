using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class FieldDtoValidator : AbstractValidator<FieldDto>
    {
        public FieldDtoValidator()
        {
            RuleFor(x => x.Label)
                .NotNull()
                .MaximumLength(Field.LabelLengthMax);

            RuleFor(x => x.Unit)
                .MaximumLength(Field.UnitLengthMax);

            RuleFor(x => x.SortKey)
                .Must(BePositive)
                .WithMessage("Sort key must be positive");

            bool BePositive(int arg) => arg > 0;
        }
    }
}
