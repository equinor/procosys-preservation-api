using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class CreateActionDtoValidator : AbstractValidator<CreateActionDto>
    {
        public CreateActionDtoValidator()
        {
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.Title)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Action.TitleLengthMax);

            RuleFor(x => x.Description)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Action.DescriptionLengthMax);
        }
    }
}
