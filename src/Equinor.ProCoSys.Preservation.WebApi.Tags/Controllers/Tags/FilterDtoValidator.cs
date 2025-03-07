using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class FilterDtoValidator : AbstractValidator<FilterDto>
    {
        public FilterDtoValidator()
        {
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.ProjectName)
                .NotNull()
                .NotEmpty();
        }
    }
}
