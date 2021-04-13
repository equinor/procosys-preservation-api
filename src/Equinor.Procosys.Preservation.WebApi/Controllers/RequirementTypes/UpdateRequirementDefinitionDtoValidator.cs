using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.RequirementTypes
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

            When(x => x.NewFields != null, () =>
            {
                RuleForEach(x => x.NewFields)
                    .Must(FieldLabelNotNullAndMaxLength)
                    .WithMessage($"Field label cannot be null and must be maximum {Field.LabelLengthMax}")
                    .Must(FieldUnitMaxLength)
                    .WithMessage($"Field unit must be maximum {nameof(Field.UnitLengthMax)}");
            });

            When(x => x.UpdatedFields != null, () =>
            {
                RuleFor(x => x.UpdatedFields)
                    .Must(BeUniqueFieldIds)
                    .WithMessage("Fields to update or delete must be unique");

                RuleForEach(x => x.UpdatedFields)
                    .Must(FieldLabelNotNullAndMaxLength)
                    .WithMessage($"Field label cannot be null and must be maximum {Field.LabelLengthMax}")
                    .Must(FieldUnitMaxLength)
                    .WithMessage($"Field unit must be maximum {nameof(Field.UnitLengthMax)}");
            });

            RuleFor(x => x)
                .Must(NotHaveDuplicateFieldLabels)
                .WithMessage("Cannot have duplicate field labels");
            
            bool BeUniqueFieldIds(IList<UpdateFieldDto> updatedFields)
            {
                var fieldIds = updatedFields.Select(u => u.Id).ToList();
                return fieldIds.Distinct().Count() == fieldIds.Count;
            }

            bool BePositive(int arg) => arg > 0;

            bool NotHaveDuplicateFieldLabels(UpdateRequirementDefinitionDto dto)
            {
                var allFieldLabelsLowercase = new List<string>();

                if (dto.UpdatedFields != null)
                {
                    allFieldLabelsLowercase.AddRange(dto.UpdatedFields.Where(f => !string.IsNullOrEmpty(f.Label))
                        .Select(f => f.Label.ToLower()));
                }

                if (dto.NewFields != null)
                {
                    allFieldLabelsLowercase.AddRange(dto.NewFields.Where(f => !string.IsNullOrEmpty(f.Label))
                        .Select(f => f.Label.ToLower()));
                }

                return allFieldLabelsLowercase.Distinct().Count() == allFieldLabelsLowercase.Count;
            }

            bool FieldLabelNotNullAndMaxLength(FieldDto fieldDto)
                => fieldDto.Label != null && fieldDto.Label.Length < Field.LabelLengthMax;

            bool FieldUnitMaxLength(FieldDto fieldDto)
                => fieldDto.Unit == null || fieldDto.Unit.Length < Field.UnitLengthMax;
        }
    }
}
