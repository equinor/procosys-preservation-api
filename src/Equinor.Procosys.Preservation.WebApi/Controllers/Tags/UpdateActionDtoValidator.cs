using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UpdateActionDtoValidator : AbstractValidator<UpdateActionDto>
    {
        public UpdateActionDtoValidator()
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
