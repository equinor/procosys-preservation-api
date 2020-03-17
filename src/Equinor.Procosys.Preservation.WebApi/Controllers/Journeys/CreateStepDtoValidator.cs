using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    public class CreateStepDtoValidator : AbstractValidator<CreateStepDto>
    {
        public CreateStepDtoValidator()
            => RuleFor(x => x.Title)
                .NotNull()
                .MinimumLength(Step.TitleLengthMin)
                .MaximumLength(Step.TitleLengthMax);
    }
}
