using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands
{
    public class RecordCommandValidator<T> : AbstractValidator<T> where T : RecordCommand
    {
        public RecordCommandValidator(ITagValidator tagValidator, IFieldValidator fieldValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command.TagId)
                .Must(NotBeInAClosedProject)
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .Must(BeAnExistingTag)
                .WithMessage(command => $"Tag doesn't exists! Tag={command.TagId}")
                .Must(NotBeAVoidedTag)
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}");

            RuleFor(command => command.FieldId)
                .Must(BeAnExistingField)
                .WithMessage(command => $"Field doesn't exists! Field={command.FieldId}")
                .Must(NotBeAVoidedField)
                .WithMessage(command => $"Field is voided! Field={command.FieldId}");

            RuleFor(command => command)
                .Must(HaveRequirementReadyForRecording)
                .WithMessage(command => $"The requirement for the field is not ready for recording! Tag={command.TagId}. Field={command.FieldId}");

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool NotBeInAClosedProject(int tagId) => !tagValidator.ProjectIsClosed(tagId);

            bool BeAnExistingField(int fieldId) => fieldValidator.Exists(fieldId);
            
            bool NotBeAVoidedField(int fieldId) => !fieldValidator.IsVoided(fieldId);
            
            bool HaveRequirementReadyForRecording(RecordCommand command)
                => tagValidator.RequirementIsReadyForRecording(command.TagId, command.FieldId);
        }
    }
}
