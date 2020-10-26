using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class RescheduleTagsDtoValidator : AbstractValidator<RescheduleTagsDto>
    {
        public RescheduleTagsDtoValidator()
        {
            RuleFor(x => x).NotNull().WithMessage($"{nameof(RescheduleTagsDto)} can't be null");

            RuleFor(x => x.TagDtos)
                .NotNull()
                .WithMessage($"{nameof(RescheduleTagsDto.TagDtos)} can't be null");
        }
    }
}
