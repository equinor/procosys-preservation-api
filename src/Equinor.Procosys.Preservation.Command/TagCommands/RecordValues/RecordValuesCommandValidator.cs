using System.Linq;
using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordValues
{
    public class RecordValuesCommandValidator : AbstractValidator<RecordValuesCommand>
    {
        public RecordValuesCommandValidator(ITagValidator tagValidator, IFieldValidator fieldValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command.TagId)
                .Must(NotBeInAClosedProject)
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .Must(BeAnExistingTag)
                .WithMessage(command => $"Tag doesn't exists! Tag={command.TagId}")
                .Must(NotBeAVoidedTag)
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}");

            RuleFor(command => command.RequirementId)
                .Must(HaveRequirementReadyForRecording)
                .WithMessage((command, reqId) =>
                    $"The requirement for the field is not ready for recording! Tag={command.TagId}. Requirement={reqId}");

            When(command => command.FieldValues.Any(), () =>
            {
                RuleForEach(command => command.FieldValues)
                    .Must(BeAValidValueForField)
                    .WithMessage((command, fv) => $"Field value is not valid! Field={fv.FieldId} Value={fv.Value}")
                    .Must(BeAnExistingField)
                    .WithMessage((command, fv) => $"Field doesn't exists! Field={fv.FieldId}")
                    .Must(NotBeAVoidedField)
                    .WithMessage((command, fv) => $"Field is voided! Field={fv.FieldId}");
            });

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool NotBeInAClosedProject(int tagId) => !tagValidator.ProjectIsClosed(tagId);

            bool BeAnExistingField(FieldValue fieldValue) => fieldValidator.Exists(fieldValue.FieldId);

            bool NotBeAVoidedField(FieldValue fieldValue) => !fieldValidator.IsVoided(fieldValue.FieldId);

            bool HaveRequirementReadyForRecording(int requirementId)
                => tagValidator.RequirementIsReadyForRecording(requirementId);

            bool BeAValidValueForField(FieldValue fieldValue)
                => fieldValidator.IsValidValue(fieldValue.FieldId, fieldValue.Value);
        }
    }
}
