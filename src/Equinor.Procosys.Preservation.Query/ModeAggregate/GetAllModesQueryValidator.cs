using FluentValidation;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetAllModesQueryValidator : AbstractValidator<GetAllModesQuery>
    {
        public GetAllModesQueryValidator()
        {
        }
    }
}
