using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Persons
{
    public class CreateSavedFilterDtoValidator : AbstractValidator<CreateSavedFilterDto>
    {
        public CreateSavedFilterDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotNull()
                .MinimumLength(SavedFilter.TitleLengthMax);
            RuleFor(x => x.Criteria)
                .NotNull()
                .MinimumLength(SavedFilter.CriteriaLengthMax);
        }
    }
}
