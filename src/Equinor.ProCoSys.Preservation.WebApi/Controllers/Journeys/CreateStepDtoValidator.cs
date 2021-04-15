using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Journeys
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
