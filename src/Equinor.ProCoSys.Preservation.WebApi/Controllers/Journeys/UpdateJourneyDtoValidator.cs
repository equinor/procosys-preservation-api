using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Journeys
{
    public class UpdateJourneyDtoValidator : AbstractValidator<UpdateJourneyDto>
    {
        public UpdateJourneyDtoValidator()
            => RuleFor(x => x.Title)
                .NotNull()
                .MinimumLength(Journey.TitleLengthMin)
                .MaximumLength(Journey.TitleLengthMax);
    }
}
