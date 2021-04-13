using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Modes
{
    public class UpdateModeDtoValidator : AbstractValidator<UpdateModeDto>
    {
        public UpdateModeDtoValidator()
            => RuleFor(x => x.Title)
                .NotNull()
                .MinimumLength(Mode.TitleLengthMin)
                .MaximumLength(Mode.TitleLengthMax);
    }
}
