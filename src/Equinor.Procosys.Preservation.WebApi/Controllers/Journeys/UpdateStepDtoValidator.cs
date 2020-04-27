using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    public class UpdateStepDtoValidator : AbstractValidator<UpdateStepDto>
    {
        public UpdateStepDtoValidator()
            => RuleFor(x => x.Title)
                .NotNull()
                .MinimumLength(Step.TitleLengthMin)
                .MaximumLength(Step.TitleLengthMax);
    }
}
