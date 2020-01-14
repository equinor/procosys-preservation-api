using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(x => x.TagNo)
                .NotEmpty()
                .MaximumLength(Tag.TagNoLengthMax);

            RuleFor(x => x.ProjectNo)
                .NotEmpty()
                .MaximumLength(Tag.ProjectNumberLengthMax);

            RuleFor(tag => tag.Requirements)
                .Must(r => r != null && r.Any())
                .WithMessage($"{nameof(CreateTagDto.Requirements)} must have at least one element")
                .Must(BeUniqueRequirements)
                .WithMessage($"{nameof(CreateTagDto.Requirements)} must be unique");

            RuleForEach(x => x.Requirements)
                .Must(RequirementMustHavePositiveInterval)
                .WithMessage($"{nameof(TagRequirementDto.IntervalWeeks)} must be positive");
            
            bool RequirementMustHavePositiveInterval(TagRequirementDto dto) => dto.IntervalWeeks > 0;

            bool BeUniqueRequirements(IEnumerable<TagRequirementDto> dtos)
            {
                if (dtos == null)
                {
                    return true;
                }

                var reqIds = dtos.Select(dto => dto.RequirementDefinitionId).ToList();
                return reqIds.Distinct().Count() == reqIds.Count;
            }
        }
    }
}
