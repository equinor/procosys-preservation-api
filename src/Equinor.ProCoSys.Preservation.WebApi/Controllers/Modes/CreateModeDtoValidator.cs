using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Modes
{
    public class CreateModeDtoValidator : AbstractValidator<CreateModeDto>
    {
        public CreateModeDtoValidator()
        {
            RuleFor(x => x.Title)
                .MinimumLength(Mode.TitleLengthMin)
                .MaximumLength(Mode.TitleLengthMax);
        }
    }
}
