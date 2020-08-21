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
                .Must(FieldMaxLength)
                .WithMessage($"Field unit must be maximum {nameof(Field.UnitLengthMax)}");

            RuleForEach(x => x.UpdatedFields)
                .Must(FieldMaxLength)
                .WithMessage($"Field unit must be maximum {nameof(Field.UnitLengthMax)}");
            
            RuleFor(x => x)
                .Must(BeUniqueFieldIds)
                .WithMessage("Fields to update or delete must be unique");
                        
            bool BeUniqueFieldIds(UpdateRequirementDefinitionDto dto)
            {
                var fieldIds = dto.UpdatedFields.Select(u => u.Id).ToList();
                fieldIds.AddRange(dto.DeleteFields.Select(u => u.Id));
                return fieldIds.Distinct().Count() == fieldIds.Count;
            }

            bool BePositive(int arg) => arg > 0;

            bool NotHaveDuplicateFieldLabels(UpdateRequirementDefinitionDto dto)
            {
                var allFieldLabelsLowercase = dto.UpdatedFields.Where(f => !string.IsNullOrEmpty(f.Label)).Select(f => f.Label.ToLower())
                    .Concat(dto.NewFields.Where(f => !string.IsNullOrEmpty(f.Label)).Select(f => f.Label.ToLower())).ToList();

                return allFieldLabelsLowercase.Distinct().Count() == allFieldLabelsLowercase.Count;
            }

            bool FieldLabelNotNullAndMaxLength(FieldDto arg) => arg.Label != null && arg.Label.Length < Field.LabelLengthMax;

            bool FieldMaxLength(FieldDto arg) => arg.Unit == null || arg.Unit.Length < Field.UnitLengthMax;
        }
    }
}
