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

            bool BeAnExistingTag(int tagId)
                => tagValidator.Exists(tagId);

            bool BeAnExistingStep(int stepId)
                => stepValidator.Exists(stepId);
        }
    }
}
