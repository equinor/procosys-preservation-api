using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class RescheduleTagsDtoValidator : AbstractValidator<RescheduleTagsDto>
    {
        public RescheduleTagsDtoValidator()
        {
            RuleFor(x => x).NotNull();

            RuleFor(x => x.TagDtos)
                .NotNull();
        }
    }
}
