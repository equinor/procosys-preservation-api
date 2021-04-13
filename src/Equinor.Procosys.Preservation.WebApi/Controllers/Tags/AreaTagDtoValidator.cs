using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class AreaTagDtoValidator : AbstractValidator<AreaTagDto>
    {
        public AreaTagDtoValidator()
        {
            RuleFor(x => x).NotNull();

            RuleFor(x => x.ProjectName).NotNull();

            RuleFor(x => x.AreaTagType).NotNull();

            RuleFor(x => x.DisciplineCode).NotNull();
        }
    }
}
