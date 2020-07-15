using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class UpdateFieldDtoValidator : AbstractValidator<UpdateFieldDto>
    {
        public UpdateFieldDtoValidator()
        {
            RuleFor(x => x.Label)
                .NotNull()
                .MaximumLength(Field.LabelLengthMax);

            RuleFor(x => x.Unit)
                .MaximumLength(Field.UnitLengthMax);

            RuleFor(x => x.SortKey)
                .NotNull()
                .WithMessage("Sort key for field cannot be null");

            RuleFor(x => x.FieldType)
                .NotNull();

            RuleFor(x => x.SortKey)
                .Must(MustBePositive)
                .WithMessage("Sort key must be positive");
        }

        private bool MustBePositive(int arg) => arg > 0;
    }
}
