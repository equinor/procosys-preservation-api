using System.Collections.Generic;
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
                .Must(BePositive)
                .WithMessage("Sort key must be positive");

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
                .WithMessage("Cannot have duplicate field labels");

            RuleForEach(x => x.NewFields)
                .Must(FieldUnitMaxLength)
                .WithMessage($"Field unit must be maximum {nameof(Field.UnitLengthMax)}");

            RuleForEach(x => x.UpdatedFields)
                .Must(FieldUnitMaxLength)
                .WithMessage($"Field unit must be maximum {nameof(Field.UnitLengthMax)}");
            
            RuleFor(x => x.UpdatedFields)
                .Must(BeUniqueFieldIds)
                .WithMessage("Fields to update or delete must be unique");
                        
            bool BeUniqueFieldIds(IList<UpdateFieldDto> updatedFields)
            {
                var fieldIds = updatedFields.Select(u => u.Id).ToList();
                return fieldIds.Distinct().Count() == fieldIds.Count;
            }

            bool BePositive(int arg) => arg > 0;

            bool NotHaveDuplicateFieldLabels(UpdateRequirementDefinitionDto dto)
            {
                var allFieldLabelsLowercase = dto.UpdatedFields.Where(f => !string.IsNullOrEmpty(f.Label)).Select(f => f.Label.ToLower())
                    .Concat(dto.NewFields.Where(f => !string.IsNullOrEmpty(f.Label)).Select(f => f.Label.ToLower())).ToList();

                return allFieldLabelsLowercase.Distinct().Count() == allFieldLabelsLowercase.Count;
            }

            bool FieldLabelNotNullAndMaxLength(FieldDto fieldDto) => fieldDto.Label != null && fieldDto.Label.Length < Field.LabelLengthMax;

            bool FieldUnitMaxLength(FieldDto fieldDto) => fieldDto.Unit == null || fieldDto.Unit.Length < Field.UnitLengthMax;
        }
    }
}
