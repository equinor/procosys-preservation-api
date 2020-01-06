using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    public class CreateJourneyDtoValidator : AbstractValidator<CreateJourneyDto>
    {
        public CreateJourneyDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotNull()
                .MinimumLength(Journey.TitleLengthMin)
                .MaximumLength(Journey.TitleLengthMax);
        }
    }
}
