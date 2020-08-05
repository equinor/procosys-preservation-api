using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class UpdateFieldDtoValidator : AbstractValidator<UpdateFieldDto>
    {
        public UpdateFieldDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .WithMessage("Id of fields to update must be included");

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
                .Must(BePositive)
                .WithMessage("Sort key must be positive");

            bool BePositive(int arg) => arg > 0;
        }
    }
}
