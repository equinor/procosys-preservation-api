using Equinor.Procosys.Preservation.Command.Validators.Step;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.SetStep
{
    public class SetStepCommandValidator : AbstractValidator<SetStepCommand>
    {
        public SetStepCommandValidator(
            ITagValidator tagValidator,
            IStepValidator stepValidator
            )
        {
            RuleFor(s => s.TagId)
                .Must(NotBeAnExistingTag)
                .WithMessage(s => $"Tag {s.TagId} don't exists");

            RuleFor(t => t.StepId)
                .Must(BeAnExistingStep)
                .WithMessage(s => $"Step {s.StepId} don't exists");

            bool NotBeAnExistingTag(int tagId)
                => !tagValidator.Exists(tagId);

            bool BeAnExistingStep(int stepId)
                => stepValidator.Exists(stepId);
        }
    }
}
