using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class RescheduleTagsDtoValidator : AbstractValidator<RescheduleTagsDto>
    {
        public RescheduleTagsDtoValidator()
        {
            RuleFor(x => x).NotNull();

            RuleFor(x => x.Tags)
                .NotNull();

            RuleFor(x => x.Comment)
                .NotNull()
                .NotEmpty();
        }
    }
}
