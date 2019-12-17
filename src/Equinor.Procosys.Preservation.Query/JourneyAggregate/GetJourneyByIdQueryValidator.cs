using FluentValidation;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class GetJourneyByIdQueryValidator : AbstractValidator<GetJourneyByIdQuery>
    {
        public GetJourneyByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan(0);
    }
}
