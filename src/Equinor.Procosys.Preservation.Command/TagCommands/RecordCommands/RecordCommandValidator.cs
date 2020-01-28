using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.FieldValue;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands
{
    public class RecordCommandValidator<T> : AbstractValidator<T> where T : RecordCommand
    {
        public RecordCommandValidator(
            ITagValidator tagValidator,
            IFieldValidator fieldValidator,
            IFieldValueValidator fieldValueValidator
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

            RuleFor(s => s.FieldId)
                .Must(BeAnExistingField)
                .WithMessage(x => $"Field doesn't exists! Field={x.FieldId}")
                .Must(NotBeAVoidedField)
                .WithMessage(x => $"Field is voided! Field={x.FieldId}")
                .Must(NotBeAnExistingFieldValueInCurrentPeriod)
                .WithMessage(x => $"Field value already exists for field in current period! Field={x.FieldId}");

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool NotBeInAClosedProject(int tagId) => !tagValidator.ProjectIsClosed(tagId);

            bool BeAnExistingField(int fieldId) => fieldValidator.Exists(fieldId);
            
            bool NotBeAVoidedField(int fieldId) => !fieldValidator.IsVoided(fieldId);
            
            bool NotBeAnExistingFieldValueInCurrentPeriod(int fieldId) => !fieldValueValidator.ExistsInCurrentPeriod(fieldId);
        }
    }
}
