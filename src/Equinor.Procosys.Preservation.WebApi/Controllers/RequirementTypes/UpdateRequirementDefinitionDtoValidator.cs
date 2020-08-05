using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class UpdateRequirementDefinitionDtoValidator : AbstractValidator<UpdateRequirementDefinitionDto>
    {
        public UpdateRequirementDefinitionDtoValidator()
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

            RuleForEach(x => x.NewFields)
                .Must(FieldLabelNotNullAndMaxLength)
                .WithMessage($"Field label cannot be null and must be maximum {Field.LabelLengthMax}");

            RuleForEach(x => x.UpdatedFields)
                .Must(FieldLabelNotNullAndMaxLength)
                .WithMessage($"Field label cannot be null and must be maximum {Field.LabelLengthMax}");

            RuleFor(x => x)
                .Must(NotHaveDuplicateFieldLabels)
                .WithMessage("Cannot have duplicate fields");

            RuleForEach(x => x.NewFields)
                .Must(FieldUnitMaxLength)
                .WithMessage($"Field unit must be maximum {nameof(Field.UnitLengthMax)}");

            RuleForEach(x => x.UpdatedFields)
                .Must(FieldUnitMaxLength)
                .WithMessage($"Field unit must be maximum {nameof(Field.UnitLengthMax)}");

            bool BePositive(int arg) => arg > 0;

            bool NotHaveDuplicateFieldLabels(UpdateRequirementDefinitionDto dto)
            {
                var allFieldLabelsLowercase = dto.UpdatedFields.Select(f => f.Label.ToLower())
                    .Concat(dto.NewFields.Select(f => f.Label.ToLower())).ToList();

                return allFieldLabelsLowercase.Distinct().Count() == allFieldLabelsLowercase.Count;
            }

            bool FieldLabelNotNullAndMaxLength(FieldDto arg) => arg.Label != null && arg.Label.Length < Field.LabelLengthMax;

            bool FieldUnitMaxLength(FieldDto arg) => arg.Unit.Length < Field.UnitLengthMax;
        }
    }
}
