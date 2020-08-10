using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Persons
{
    public class CreateSavedFilterDtoValidator : AbstractValidator<CreateSavedFilterDto>
    {
        public CreateSavedFilterDtoValidator()
        {
            RuleFor(x => x.ProjectName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Project.NameLengthMax);
            RuleFor(x => x.Title)
                .NotNull()
                .MaximumLength(SavedFilter.TitleLengthMax);
            RuleFor(x => x.Criteria)
                .NotNull()
                .MaximumLength(SavedFilter.CriteriaLengthMax);
        }
    }
}
