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
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.TagId)
                .Must(NotBeInAClosedProject)
                .WithMessage(x => $"Project for tag is closed! Tag={x.TagId}")
                .Must(BeAnExistingTag)
                .WithMessage(x => $"Tag doesn't exists! Tag={x.TagId}")
                .Must(NotBeAVoidedTag)
                .WithMessage(x => $"Tag is voided! Tag={x.TagId}");

            RuleFor(s => s.StepId)
                .Must(BeAnExistingStep)
                .WithMessage(x => $"Step doesn't exists! Step={x.StepId}")
                .Must(NotBeAVoidedStep)
                .WithMessage(x => $"Step is voided! Step={x.StepId}");

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool NotBeInAClosedProject(int tagId) => !tagValidator.ProjectIsClosed(tagId);

            bool BeAnExistingStep(int stepId) => stepValidator.Exists(stepId);
            
            bool NotBeAVoidedStep(int stepId) => !stepValidator.IsVoided(stepId);
        }
    }
}
