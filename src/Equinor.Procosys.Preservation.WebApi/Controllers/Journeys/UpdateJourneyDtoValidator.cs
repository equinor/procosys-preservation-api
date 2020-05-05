using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
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
