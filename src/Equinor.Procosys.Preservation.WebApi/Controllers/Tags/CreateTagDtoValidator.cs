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

            RuleFor(tag => tag.RequirementDefinitionIds)
                .NotNull();

            RuleFor(tag => tag.RequirementDefinitionIds.Count())
                .GreaterThan(1);
        }
    }
}
