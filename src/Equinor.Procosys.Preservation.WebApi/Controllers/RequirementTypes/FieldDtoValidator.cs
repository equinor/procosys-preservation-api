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
                .NotNull()
                .WithMessage("Sort key for field cannot be null");

            RuleFor(x => x.FieldType)
                .NotNull();
        }
    }
}
