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
                .MaximumLength(Tag.ProjectNoLengthMax);

            RuleFor(tag => tag.Requirements)
                .Must(r => r != null && r.Any())
                .WithMessage($"{nameof(CreateTagDto.Requirements)} must have at least one element")
                .Must(BuUniqueRequirements)
                .WithMessage($"{nameof(CreateTagDto.Requirements)} must be unique");

            RuleForEach(x => x.Requirements)
                .Must(RequirementMustHaveInterval)
                .WithMessage($"{nameof(TagRequirementDto.Interval)} must be positive");
            
            bool RequirementMustHaveInterval(TagRequirementDto dto) => dto.Interval > 0;

            bool BuUniqueRequirements(IEnumerable<TagRequirementDto> dtos)
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
