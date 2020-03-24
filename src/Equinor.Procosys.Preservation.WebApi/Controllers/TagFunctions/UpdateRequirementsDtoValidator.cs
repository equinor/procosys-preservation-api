using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.TagFunctions
{
    public class UpdateRequirementsValidator : AbstractValidator<UpdateRequirementsDto>
    {
        public UpdateRequirementsValidator()
        {
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.TagFunctionCode)
                .NotNull()
                .NotEmpty()
                .MaximumLength(TagFunction.CodeLengthMax);

            RuleFor(x => x.RegisterCode)
                .NotNull()
                .NotEmpty()
                .MaximumLength(TagFunction.RegisterCodeLengthMax);

            RuleFor(x => x.Requirements)
                .Must(BeUniqueRequirements)
                .WithMessage("Requirement definitions must be unique!");

            RuleForEach(x => x.Requirements)
                .Must(RequirementMustHavePositiveInterval)
                .WithMessage($"{nameof(TagFunctionRequirementDto.IntervalWeeks)} must be positive");

            bool RequirementMustHavePositiveInterval(TagFunctionRequirementDto dto) => dto.IntervalWeeks > 0;
                        
            bool BeUniqueRequirements(IEnumerable<TagFunctionRequirementDto> requirements)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return reqIds.Distinct().Count() == reqIds.Count;
            }
        }
    }
}
