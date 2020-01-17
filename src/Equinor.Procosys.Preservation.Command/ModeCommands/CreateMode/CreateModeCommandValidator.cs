using Equinor.Procosys.Preservation.Command.Validators.Mode;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode
{
    public class CreateModeCommandValidator : AbstractValidator<CreateModeCommand>
    {
        public CreateModeCommandValidator(IModeValidator modeValidator)
        {
            RuleFor(x => x.Title)
                .Must(HaveUniqueTitle)
                .WithMessage(x => $"Mode with title already exists! Mode={x.Title}");

            bool HaveUniqueTitle(string title) => !modeValidator.Exists(title);
        }
    }
}
