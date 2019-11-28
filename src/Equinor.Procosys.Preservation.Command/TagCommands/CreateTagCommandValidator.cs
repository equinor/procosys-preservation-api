using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagCommandValidator()
        {
            RuleFor(x => x.ProjectNo).NotEmpty();
            RuleFor(x => x.Schema).NotEmpty();
            RuleFor(x => x.TagNo).NotEmpty();
        }
    }
}
