using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Step;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Command.Validators.Project;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.SetStep
{
    public class SetStepCommandValidator : AbstractValidator<SetStepCommand>
    {
        public SetStepCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator,
            IStepValidator stepValidator
            )
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .Must(command => BeAnExistingTag(command.TagId))
                .WithMessage(command => $"Tag doesn't exists! Tag={command.TagId}")
                .Must(command => NotBeAVoidedTag(command.TagId))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}");

            RuleFor(s => s.StepId)
                .Must(BeAnExistingStep)
                .WithMessage(x => $"Step doesn't exists! Step={x.StepId}")
                .Must(NotBeAVoidedStep)
                .WithMessage(x => $"Step is voided! Step={x.StepId}");
                        
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken cancellationToken)
                => !await projectValidator.IsClosedForTagAsync(tagId, cancellationToken);

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool BeAnExistingStep(int stepId) => stepValidator.Exists(stepId);
            
            bool NotBeAVoidedStep(int stepId) => !stepValidator.IsVoided(stepId);
        }
    }
}
