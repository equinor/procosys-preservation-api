using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Modes
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
