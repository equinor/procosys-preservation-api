using FluentValidation;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetModeByIdQueryValidator : AbstractValidator<GetModeByIdQuery>
    {
        public GetModeByIdQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
