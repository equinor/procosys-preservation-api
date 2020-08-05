using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class CreateRequirementDefinitionDtoValidator : AbstractValidator<CreateRequirementDefinitionDto>
    {
        public CreateRequirementDefinitionDtoValidator()
        {
            RuleFor(x => x).NotNull();

            RuleFor(x => x.Title)
                .MaximumLength(RequirementDefinition.TitleLengthMax);

            RuleFor(x => x.SortKey)
                .NotNull()
                .WithMessage("Sort key cannot be null")
                .Must(BePositive)
                .WithMessage("Sort key must be positive");

            RuleFor(x => x.Usage)
                .NotNull()
                .WithMessage("Usage cannot be null");

            RuleFor(x => x.DefaultIntervalWeeks)
                .Must(BePositive)
                .WithMessage("Week interval must be positive");

            RuleForEach(x => x.Fields)
                .Must(FieldLabelNotNullAndMaxLength)
                .WithMessage($"Field label cannot be null and must be maximum {nameof(Field.LabelLengthMax)}");

            RuleFor(x => x.Fields)
                .Must(NotBeDuplicates)
                .WithMessage("Cannot have duplicate fields");

            RuleForEach(x => x.Fields)
                .Must(FieldUnitMaxLength)
                .WithMessage($"Field unit must be maximum {nameof(Field.UnitLengthMax)}");

            bool BePositive(int arg) => arg > 0;

            bool NotBeDuplicates(IList<FieldDto> fields)
            {
                var lowerCaseField = fields.Select(f => f.Label.ToLower()).ToList();

                return lowerCaseField.Distinct().Count() == lowerCaseField.Count;
            }

            bool FieldLabelNotNullAndMaxLength(FieldDto arg) => arg.Label != null && arg.Label.Length < Field.LabelLengthMax;

            bool FieldUnitMaxLength(FieldDto arg) => arg.Unit.Length < Field.UnitLengthMax;
        }
    }
}
