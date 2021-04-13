using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Journeys
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
