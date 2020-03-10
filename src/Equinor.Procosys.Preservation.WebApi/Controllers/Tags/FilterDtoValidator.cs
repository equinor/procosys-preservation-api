using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class FilterDtoValidator : AbstractValidator<FilterDto>
    {
        public FilterDtoValidator()
        {
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.ProjectName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Project.NameLengthMax);
        }
    }
}
