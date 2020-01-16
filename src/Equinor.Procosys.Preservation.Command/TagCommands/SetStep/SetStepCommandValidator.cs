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
                .Must(BeAnExistingTag)
                .WithMessage(s => $"Tag doesn't exists! Tag={s.TagId}");

            RuleFor(s => s.StepId)
                .Must(BeAnExistingStep)
                .WithMessage(s => $"Step doesn't exists! Step={s.StepId}");

            RuleFor(x => x.TagId)
                .Must(NotBeAVoidedTag)
                .WithMessage(x => $"Tag is voided! Tag={x.TagId}");

            RuleFor(x => x.StepId)
                .Must(NotBeAVoidedStep)
                .WithMessage(x => $"Step is voided! Step={x.StepId}");

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool BeAnExistingStep(int stepId) => stepValidator.Exists(stepId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);
            
            bool NotBeAVoidedStep(int stepId) => !stepValidator.IsVoided(stepId);
        }
    }
}
