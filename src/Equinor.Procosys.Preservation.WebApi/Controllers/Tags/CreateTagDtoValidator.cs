using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(Tag.DescriptionLengthMax);

            RuleFor(x => x.TagNo)
                .NotEmpty()
                .MaximumLength(Tag.TagNumberLengthMax);

            RuleFor(x => x.ProjectNo)
                .NotEmpty()
                .MaximumLength(Tag.ProjectNumberLengthMax);
        }
    }
}
