using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Modes
{
    public class CreateModeDtoValidator : AbstractValidator<CreateModeDto>
    {
        public CreateModeDtoValidator()
        {
            RuleFor(x => x.Title)
                .MinimumLength(Mode.TitleMinLength)
                .MaximumLength(Mode.TitleLengthMax);
        }
    }
}
