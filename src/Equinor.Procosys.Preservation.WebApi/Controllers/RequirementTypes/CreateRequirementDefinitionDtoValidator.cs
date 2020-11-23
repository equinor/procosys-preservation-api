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
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.Title)
                .MaximumLength(RequirementDefinition.TitleLengthMax);

            RuleFor(x => x.SortKey)
                .Must(BePositive)
                .WithMessage("Sort key must be positive");

            RuleFor(x => x.DefaultIntervalWeeks)
                .Must(BePositive)
                .WithMessage("Week interval must be positive");

            When(x => x.Fields != null, () =>
            {
                RuleFor(x => x.Fields)
                    .Must(NotHaveDuplicateFieldLabels)
                    .WithMessage("Cannot have duplicate fields");

                RuleForEach(x => x.Fields)
                    .Must(FieldLabelNotNullAndMaxLength)
                    .WithMessage($"Field label cannot be null and must be maximum {nameof(Field.LabelLengthMax)}")
                    .Must(FieldUnitMaxLength)
                    .WithMessage($"Field unit must be maximum {nameof(Field.UnitLengthMax)}");
            });

            bool BePositive(int arg) => arg > 0;

            bool NotHaveDuplicateFieldLabels(IList<FieldDto> fields)
            {
                var lowerCaseFieldLabels = fields.Select(f => f.Label.ToLower()).ToList();

                return lowerCaseFieldLabels.Distinct().Count() == lowerCaseFieldLabels.Count;
            }

            bool FieldLabelNotNullAndMaxLength(FieldDto fieldDto)
                => fieldDto.Label != null && fieldDto.Label.Length < Field.LabelLengthMax;

            bool FieldUnitMaxLength(FieldDto fieldDto)
                => fieldDto.Unit == null || fieldDto.Unit.Length < Field.UnitLengthMax;
        }
    }
}
